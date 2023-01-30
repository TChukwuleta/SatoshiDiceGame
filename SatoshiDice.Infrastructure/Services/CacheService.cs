using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Common.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SatoshiDice.Infrastructure.Services
{
    public class CacheService : ICacheService
    {
        private IConnectionMultiplexer _redis;
        private IDatabase _database;
        private readonly IConfiguration _config;
        public CacheService(IConnectionMultiplexer redis, IConfiguration config)
        {
            _redis = redis;
            _config = config;
            _database = _redis.GetDatabase();
        }

        public async Task<IEnumerable<T>> GetData<T>(string key)
        {
            try
            {
                var TKey = _config[$"RedisDatabaseKey:{key}"];
                var data = await _database.HashGetAllAsync(TKey);
                if(data.Length > 0)
                {
                    var obj = Array.ConvertAll(data, val => JsonSerializer.Deserialize<T>(val.Value)).ToString();
                    return (IEnumerable<T>)JsonSerializer.Deserialize<T>(obj);
                }
                return default;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<T> GetDataById<T>(string key, string id)
        {
            try
            {
                var TKey = _config[$"RedisDatabaseKey:{key}"];
                var data = await _database.HashGetAsync(TKey, id);
                if (!string.IsNullOrEmpty(data))
                {
                    return JsonSerializer.Deserialize<T>(data);
                }
                return default;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<object> RemoveData(string key)
        {
            try
            {
                var TKey = _config[$"RedisDatabaseKey:{key}"];
                var exist = await _database.KeyExistsAsync(TKey);
                if (exist)
                {
                    return await _database.KeyDeleteAsync(key);
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SetData<T>(string key, T value, string id)
        {
            try
            {
                var TKey = _config[$"RedisDatabaseKey:{key}"];
                var serializedData = JsonSerializer.Serialize<T>(value);
                _database.HashSet(TKey, new HashEntry[]
                {
                    new HashEntry(id, serializedData)
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
