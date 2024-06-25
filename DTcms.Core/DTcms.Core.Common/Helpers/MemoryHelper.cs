using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DTcms.Core.Common.Helpers
{
    /// <summary>
    /// 缓存帮助类
    /// </summary>
    public class MemoryHelper
    {
        private static readonly MemoryCache Cache = new(new MemoryCacheOptions());

        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static bool Exists(string? key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return Cache.TryGetValue(key, out _);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">滑动过期时长（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <param name="expiressAbsoulte">绝对过期时长</param>
        /// <returns></returns>
        public static bool Set(string? key, object? value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(value);

            Cache.Set(key, value,
                new MemoryCacheEntryOptions().SetSlidingExpiration(expiresSliding)
                    .SetAbsoluteExpiration(expiressAbsoulte));
            return Exists(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresIn">缓存时长</param>
        /// <param name="isSliding">是否滑动过期（如果在过期时间内有操作，则以当前时间点延长过期时间）</param>
        /// <returns></returns>
        public static bool Set(string? key, object? value, TimeSpan expiresIn, bool isSliding = false)
        {
            ArgumentNullException.ThrowIfNull(key);
            if (value == null)  throw new ArgumentNullException(nameof(value));

            Cache.Set(key, value,
                isSliding
                    ? new MemoryCacheEntryOptions().SetSlidingExpiration(expiresIn)
                    : new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiresIn));

            return Exists(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static void Remove(string? key)
        {
            ArgumentNullException.ThrowIfNull(key);

            Cache.Remove(key);
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <returns></returns>
        public static void RemoveAll(IEnumerable<string>? keys)
        {
            ArgumentNullException.ThrowIfNull(keys);

            keys.ToList().ForEach(item => Cache.Remove(item));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static T? Get<T>(string? key) where T : class
        {
            ArgumentNullException.ThrowIfNull(key);
            return Cache.Get(key) as T;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public static object? Get(string? key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return Cache.Get(key);
        }

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public static IDictionary<string, object?> GetAll(IEnumerable<string>? keys)
        {
            ArgumentNullException.ThrowIfNull(keys);

            var dict = new Dictionary<string, object?>();
            keys.ToList().ForEach(item => dict.Add(item, Cache.Get(item)));
            return dict;
        }

        /// <summary>
        /// 删除所有缓存
        /// </summary>
        public static void RemoveCacheAll()
        {
            var list = GetCacheKeys();
            if (list != null)
            {
                foreach (var s in list)
                {
                    Remove(s);
                }
            }
        }

        /// <summary>
        /// 删除匹配到的缓存
        /// </summary>
        public static void RemoveCacheRegex(string pattern)
        {
            var list = SearchCacheRegex(pattern);
            if (list != null)
            {
                foreach (var s in list)
                {
                    Remove(s);
                }
            }
        }

        /// <summary>
        /// 搜索匹配到的缓存
        /// </summary>
        public static IList<string>? SearchCacheRegex(string pattern)
        {
            var cacheKeys = GetCacheKeys();
            var l = cacheKeys?.Where(k => Regex.IsMatch(k, pattern)).ToList();
            return l?.AsReadOnly();
        }

        /// <summary>
        /// 获取所有缓存键
        /// </summary>
        public static List<string>? GetCacheKeys()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            var entries = Cache.GetType()?.GetField("_entries", flags)?.GetValue(Cache);
            var cacheItems = entries as IDictionary;
            var keys = new List<string>();
            if (cacheItems == null) return keys;
            foreach (DictionaryEntry item in cacheItems)
            {
                var keyValue = item.Key.ToString();
                if (keyValue != null)
                {
                    keys.Add(keyValue);
                }
            }
            return keys;
        }

    }
}