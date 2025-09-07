using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Database
{
    public class RedisProxy : IRedisProvider
    {
        private readonly IDistributedCache _distributedCache;

        public RedisProxy(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public virtual async Task<TValue?> LoadAsync<TValue>(RedisKey keyType, object[] keyIds)
            where TValue : class
        {
            TValue? value = null;
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();

            string? valueAsString = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrWhiteSpace(valueAsString))
            {
                value = JsonSerializer.Deserialize<TValue>(valueAsString);
            }

            return value;
        }

        public virtual async Task UpdateAsync<TValue>(RedisKey keyType, TValue value, object[] keyIds)
            where TValue : class
        {
            if (value is null)
                throw new InvalidOperationException();

            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            string valueAsString = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, valueAsString);
        }

        public virtual async Task DeleteAsync(RedisKey keyType, object[] keyIds)
        {
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            await _distributedCache.RemoveAsync(key);
        }

        public virtual async Task<TValue?> LoadOrCreateAsync<TValue>(RedisKey keyType, Func<Task<(TValue Value, DistributedCacheEntryOptions EntryOptions)>> loadAction, object[] keyIds)
            where TValue : class
        {
            TValue? value = null;
            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();

            string? valueAsString = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrWhiteSpace(valueAsString))
            {
                value = JsonSerializer.Deserialize<TValue>(valueAsString);
            }

            if (value is null)
            {
                (value, DistributedCacheEntryOptions entryOptions) = await loadAction();

                // if entryOptions is null => the value should not be cached
                if (value is not null && entryOptions is not null)
                {
                    valueAsString = JsonSerializer.Serialize(value);

                    await _distributedCache.SetStringAsync(key, valueAsString, entryOptions);
                }
            }

            return value;
        }

        public virtual async Task AddOrUpdateAsync<TValue>(RedisKey keyType, TValue value, DistributedCacheEntryOptions entryOptions, object[] keyIds)
            where TValue : class
        {
            if (value is null)
                throw new InvalidOperationException();

            string key = new RedisKeyBuilder(keyType, keyIds).BuildKey();
            string valueAsString = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, valueAsString, entryOptions);
        }
    }
}