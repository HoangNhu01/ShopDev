using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MB.Utils.Cache
{
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// Set cache theo key value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(this IDistributedCache cache, string key, T value)
        {
            Set(cache, key, value, new DistributedCacheEntryOptions());
        }

        /// <summary>
        /// Set cache hết hạn theo thời gian tính bằng giây
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds">Thời gian hết hạn</param>
        /// <returns></returns>
        public static void Set<T>(this IDistributedCache cache, string key, T value, int seconds)
        {
            var options = new DistributedCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromSeconds(seconds));
            Set(cache, key, value, options);
        }

        public static void Set<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options
        )
        {
            var bytes = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(value, GetJsonSerializeOptions())
            );
            cache.Set(key, bytes, options);
        }

        /// <summary>
        /// Set cache theo key value với thời gian hết hạn mặc định
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task SetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            return SetAsync(cache, key, value, new DistributedCacheEntryOptions());
        }

        /// <summary>
        /// Set cache hết hạn theo thời gian tính bằng giây (thời gian tuyệt đối)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds">Thời gian hết hạn</param>
        /// <returns></returns>
        public static Task SetAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            int seconds
        )
        {
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds));
            return SetAsync(cache, key, value, options);
        }

        public static Task SetAsync<T>(
            this IDistributedCache cache,
            string key,
            T value,
            DistributedCacheEntryOptions options
        )
        {
            var bytes = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(value, GetJsonSerializeOptions())
            );
            return cache.SetAsync(key, bytes, options);
        }

        public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
        {
            var val = cache.Get(key);
            value = default;

            if (val == null)
                return false;
            value = JsonSerializer.Deserialize<T>(val, GetJsonSerializeOptions());
            return true;
        }

        public static async Task<T?> GetValueAsync<T>(this IDistributedCache cache, string key)
        {
            var val = await cache.GetAsync(key);
            if (val == null)
                return default;

            T? value = JsonSerializer.Deserialize<T>(val, GetJsonSerializeOptions());

            return value;
        }

        private static JsonSerializerOptions GetJsonSerializeOptions()
        {
            return new JsonSerializerOptions()
            {
                PropertyNamingPolicy = null,
                WriteIndented = false,
                AllowTrailingCommas = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }

        /// <summary>
        /// Xoá key theo pattern
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="connection"></param>
        /// <param name="pattern"></param>
        public static void RemoveByPattern(this IDistributedCache cache, ConnectionMultiplexer connection, string pattern)
        {
            var endPoints = connection.GetEndPoints();
            IServer? server = null;
            foreach (var endPoint in endPoints)
            {
                var dnsEndPoint = endPoint as System.Net.DnsEndPoint;
                if (dnsEndPoint == null)
                {
                    continue;
                }
                server = connection.GetServer(dnsEndPoint.Host, dnsEndPoint.Port);
                if (server.IsConnected)
                {
                    break;
                }
            }
            if (server == null)
            {
                return;
            }
            var keys = server.Keys(pattern: pattern);
            foreach (var key in keys)
            {
                cache.Remove(key);
            }
        }

        /// <summary>
        /// Xoá key theo pattern
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="connection"></param>
        /// <param name="pattern"></param>
        public static async Task RemoveByPatternAsync(this IDistributedCache cache, ConnectionMultiplexer connection, string pattern)
        {
            var endPoints = connection.GetEndPoints();
            IServer? server = null;
            foreach (var endPoint in endPoints)
            {
                var dnsEndPoint = endPoint as System.Net.DnsEndPoint;
                if (dnsEndPoint == null)
                {
                    continue;
                }
                server = connection.GetServer(dnsEndPoint.Host, dnsEndPoint.Port);
                if (server.IsConnected)
                {
                    break;
                }
            }
            if (server == null)
            {
                return;
            }
            var keys = server.KeysAsync(pattern: pattern);
            await foreach (var key in keys)
            {
                await cache.RemoveAsync(key);
            }
        }
    }
}
