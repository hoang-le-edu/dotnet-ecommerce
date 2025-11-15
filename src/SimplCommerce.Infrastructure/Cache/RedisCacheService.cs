using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimplCommerce.Infrastructure.Cache
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _redis;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly bool _isEnabled;
        private readonly string _instanceName;

        public RedisCacheService(
            IDistributedCache distributedCache,
            IConnectionMultiplexer redis,
            IConfiguration configuration,
            ILogger<RedisCacheService> logger)
        {
            _distributedCache = distributedCache;
            _redis = redis;
            _configuration = configuration;
            _logger = logger;
            _isEnabled = configuration.GetValue<bool>("Redis:Enabled");
            _instanceName = configuration.GetValue<string>("Redis:InstanceName") ?? "SimplCommerce_";
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (!_isEnabled)
            {
                _logger.LogDebug("Redis is disabled. Skipping cache get for key: {Key}", key);
                return default(T);
            }

            try
            {
                var fullKey = $"{_instanceName}{key}";
                _logger.LogInformation("Redis GET: {Key}", fullKey);

                var value = await _distributedCache.GetStringAsync(fullKey);

                if (string.IsNullOrEmpty(value))
                {
                    _logger.LogInformation("Redis MISS: {Key}", fullKey);
                    return default(T);
                }

                _logger.LogInformation("Redis HIT: {Key}", fullKey);
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis GET Error for key: {Key}. Returning default value.", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (!_isEnabled)
            {
                _logger.LogDebug("Redis is disabled. Skipping cache set for key: {Key}", key);
                return;
            }

            try
            {
                var fullKey = $"{_instanceName}{key}";
                var expirationTime = expiration ?? TimeSpan.FromMinutes(60);
                _logger.LogInformation("Redis SET: {Key}, Expiration: {Minutes} minutes",
                    fullKey, expirationTime.TotalMinutes);

                var serializedValue = JsonSerializer.Serialize(value);

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expirationTime
                };

                await _distributedCache.SetStringAsync(fullKey, serializedValue, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis SET Error for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (!_isEnabled)
            {
                _logger.LogDebug("Redis is disabled. Skipping cache remove for key: {Key}", key);
                return;
            }

            try
            {
                var fullKey = $"{_instanceName}{key}";
                _logger.LogInformation("Redis REMOVE: {Key}", fullKey);
                await _distributedCache.RemoveAsync(fullKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis REMOVE Error for key: {Key}", key);
            }
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            if (!_isEnabled)
            {
                _logger.LogDebug("Redis is disabled. Skipping cache remove by prefix: {Prefix}", prefix);
                return;
            }

            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints()[0]);
                var pattern = $"{_instanceName}{prefix}*";
                _logger.LogInformation("Redis REMOVE BY PREFIX: {Pattern}", pattern);

                await foreach (var key in server.KeysAsync(pattern: pattern))
                {
                    var keyString = key.ToString();
                    if (keyString.StartsWith(_instanceName))
                    {
                        keyString = keyString.Substring(_instanceName.Length);
                    }
                    await _distributedCache.RemoveAsync(keyString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis REMOVE BY PREFIX Error for prefix: {Prefix}", prefix);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (!_isEnabled)
            {
                return false;
            }

            try
            {
                var fullKey = $"{_instanceName}{key}";
                var value = await _distributedCache.GetStringAsync(fullKey);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis EXISTS Error for key: {Key}", key);
                return false;
            }
        }
    }
}
