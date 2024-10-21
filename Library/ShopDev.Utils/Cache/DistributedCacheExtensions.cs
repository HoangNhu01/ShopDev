using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace ShopDev.Utils.Cache
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
        public static void RemoveByPattern(
            this IDistributedCache cache,
            ConnectionMultiplexer connection,
            string pattern
        )
        {
            var endPoints = connection.GetEndPoints();
            var database = connection.GetDatabase();
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
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                cache.Remove(key!);
            }
        }

        /// <summary>
        /// Xoá key theo pattern
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="connection"></param>
        /// <param name="pattern"></param>
        public static async Task RemoveByPatternAsync(
            this IDistributedCache cache,
            IConnectionMultiplexer connection,
            string pattern
        )
        {
            var endPoints = connection.GetEndPoints();
            IServer? server = null;
            foreach (var endPoint in endPoints)
            {
                DnsEndPoint? dnsEndPoint = endPoint as DnsEndPoint;
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
                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }
                await cache.RemoveAsync(key!);
            }
        }

        /// <summary>
        /// Tạo khóa phân tán
        /// </summary>
        /// <param name="database">database Redis</param>
        /// <param name="lockKey">Định danh khóa</param>
        /// <param name="lockValue">Giá trị khóa</param>
        /// <param name="expiration">Thời gian hết hạn</param>
        /// <returns></returns>
        public static async Task<bool> AcquireLockAsync(
            this IDatabase database,
            string lockKey,
            string lockValue,
            TimeSpan expiration
        )
        {
            /*
              - Khi StringSetAsync được gọi với When.NotExists, Redis sẽ kiểm tra xem khóa (lockKey) đã tồn tại hay chưa.
               + Nếu khóa chưa tồn tại, Redis sẽ thiết lập giá trị của khóa thành lockValue và thiết lập thời gian hết hạn của khóa.
               + Nếu khóa đã tồn tại, Redis sẽ không thay đổi giá trị hiện tại và trả về false.
            */
            return await database.StringSetAsync(lockKey, lockValue, expiration, When.NotExists);
        }

        /// <summary>
        /// Tháo khóa
        /// </summary>
        /// <param name="database"></param>
        /// <param name="lockKey"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        public static async Task<bool> ReleaseLockAsync(
            this IDatabase database,
            string lockKey,
            string lockValue
        )
        {
            string luaScript =
                @"
            if redis.call('get', KEYS[1]) == ARGV[1] then
                return redis.call('del', KEYS[1])
            else
                return 0
            end";
            try
            {
                int result = (int)
                    await database.ScriptEvaluateAsync(luaScript, [lockKey], [lockValue]);
                return result == 1;
            }
            catch
            {
                return false;
            }
        }
    }
}
