using DTcms.Core.Common.Helpers;
using DTcms.Core.IServices;
using DTcms.Core.Model.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Collections;

namespace DTcms.Core.Services
{
    public class CacheService(IDistributedCache cache, IOptions<CacheSettingsDto> cacheConfig) : ICacheService
    {
        private readonly IDistributedCache _cache = cache;
        private readonly CacheSettingsDto _cacheConfig = cacheConfig.Value;
        private readonly SemaphoreSlim _lock = new(1, 1);

        /// <summary>
        /// 读取或写入缓存
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="retrieveFunc">回调函数</param>
        /// <param name="expiry">过期时间</param>
        public async Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T?>> retrieveFunc, TimeSpan? expiry = null)
        {
            if (_cacheConfig.Enabled != true)
            {
                // 如果没有启用缓存，直接检索数据
                return await retrieveFunc();
            }

            // 尝试从缓存中获取数据
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonHelper.ToJson<T>(cachedData);
            }

            await _lock.WaitAsync();  //等待锁
            try
            {
                //双重检查，防止缓存击穿
                cachedData = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return JsonHelper.ToJson<T>(cachedData);
                }

                // 从源获取数据
                var data = await retrieveFunc();
                if (data == null || (data is ICollection list && list.Count == 0))
                {
                    return default;
                }
                var serializedData = JsonHelper.ToJson(data);
                var cacheEntryOptions = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                {
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = expiry; //设置缓存的过期时间
                }
                else
                {
                    cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1); //默认设置1天
                }
                //如果 expiry 为 null，不设置 AbsoluteExpirationRelativeToNow，缓存项将永不过期
                await _cache.SetStringAsync(cacheKey, serializedData, cacheEntryOptions);

                return data;
            }
            finally
            {
                _lock.Release();  //释放锁
            }
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="data">T对象</param>
        /// <param name="expiry">过期时间</param>
        public async Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiry = null)
        {
            if (data == null) return;
            var serializedData = JsonHelper.ToJson(data);
            var cacheEntryOptions = new DistributedCacheEntryOptions();
            if (expiry.HasValue)
            {
                cacheEntryOptions.AbsoluteExpirationRelativeToNow = expiry;
            }
            else
            {
                cacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1); //默认设置1天
            }
            // 如果 expiry 为 null，不设置 AbsoluteExpirationRelativeToNow，缓存项将永不过期
            await _cache.SetStringAsync(cacheKey, serializedData, cacheEntryOptions);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        public async Task RemoveAsync(string cacheKey)
        {
            if (_cacheConfig.Enabled != true) return;
            await _cache.RemoveAsync(cacheKey);
        }

        /// <summary>
        /// 根据条件删除缓存
        /// </summary>
        /// <param name="pattern">表达式如:"Article:List:*"</param>
        public async Task RemovePatternAsync(string pattern)
        {
            if (!_cacheConfig.Enabled) return;
            pattern = $"{_cacheConfig.InstanceName}{pattern}";

            MemoryHelper.RemoveCacheRegex(pattern);
            await Task.CompletedTask;
        }
    }
}
