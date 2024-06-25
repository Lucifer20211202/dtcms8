using System;
using System.Collections.Generic;
using System.Text;

namespace DTcms.Core.IServices
{
    public interface ICacheService
    {
        /// <summary>
        /// 读取或写入缓存
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="retrieveData">回调函数</param>
        /// <param name="expiry">过期时间</param>
        Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T?>> retrieveFunc, TimeSpan? expiry = null);

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <typeparam name="T">T对象</typeparam>
        /// <param name="cacheKey">缓存Key</param>
        /// <param name="data">T对象</param>
        /// <param name="expiry">过期时间</param>
        Task SetAsync<T>(string cacheKey, T data, TimeSpan? expiry = null);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="cacheKey">缓存Key</param>
        Task RemoveAsync(string cacheKey);

        /// <summary>
        /// 根据条件删除缓存
        /// </summary>
        /// <param name="pattern">表达式如:"Article:List:*"</param>
        Task RemovePatternAsync(string pattern);
    }
}
