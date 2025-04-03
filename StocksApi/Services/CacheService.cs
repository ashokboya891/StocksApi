using StackExchange.Redis;
using StocksApi.IServiceContracts;
using System.Text.Json;

namespace StocksApi.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;

        public CacheService(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var data = await _cache.StringGetAsync(key);
            return data.HasValue ? JsonSerializer.Deserialize<T>(data) : default;
        }

        public async Task SetDataAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonData = JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, jsonData, expiration);
        }

        public async Task RemoveDataAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }
}
