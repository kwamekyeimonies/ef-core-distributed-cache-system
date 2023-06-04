using System.Text.Json;
using StackExchange.Redis;

namespace Distributed_Cache.Services
{
    public class CacheService : ICacheService
    {
        private IDatabase? _cacheDb;

        // Connect to Redis Database
        public CacheService()
        {
            var Redis_Connection = ConnectionMultiplexer.Connect("localhost:6379");
            _cacheDb = Redis_Connection.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
            {
                return _cacheDb.KeyDelete(key);
            }

            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expiratinTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expiratinTime);
        }
    }
}