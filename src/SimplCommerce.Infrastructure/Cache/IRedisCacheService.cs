using System;
using System.Threading.Tasks;

namespace SimplCommerce.Infrastructure.Cache
{
    public interface IRedisCacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task RemoveByPrefixAsync(string prefix);
        Task<bool> ExistsAsync(string key);
    }
}
