using Application.Features.Helper;
using Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Cache
{
    public class CacheService : ICacheService
    {
        private IDatabase _db;
        private readonly IDistributedCache _distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            distributedCache = _distributedCache;   
            ConfigureRedis();
        }
        private void ConfigureRedis()
        {
            _db = ConnectionHelper.Connection.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }
        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            var options = new DistributedCacheEntryOptions();
            if (expirationTime.HasValue)
            {
                options.SetAbsoluteExpiration(expirationTime.Value);
            }
            await _distributedCache.SetStringAsync(key, serializedValue, options);
        }
        public async Task<T> GetAsync<T>(string key)
        {

            var value = await _distributedCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }
    }
}
