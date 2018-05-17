using MF.Core.Redis;
using MF.Core.Utilities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Caching
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly IDatabase _db;
        private readonly RedisManager _redisManager;

        public RedisCacheManager(RedisManager redisManager, int dbSpace)
        {
            _redisManager = redisManager;
            this._db = redisManager.muxer.GetDatabase(dbSpace);
        }


        #region Utilities

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Key of cached item</param>
        /// <returns>The cached value associated with the specified key</returns>
        protected virtual async Task<T> GetAsync<T>(string key)
        {
            //little performance workaround here:
            //we use "PerRequestCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            //if (_perRequestCacheManager.IsSet(key))
            //    return _perRequestCacheManager.Get<T>(key);

            //get serialized item from cache
            var serializedItem = await _db.StringGetAsync(key);
            if (!serializedItem.HasValue)
                return default(T);

            //deserialize item
            var item = JsonConvert.DeserializeObject<T>(serializedItem);
            if (item == null)
                return default(T);

            //set item in the per-request cache
            //_perRequestCacheManager.Set(key, item, 0);

            return item;
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        protected virtual async Task SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            //set cache time
            var expiresIn = TimeSpan.FromMinutes(cacheTime);

            //serialize item
            var serializedItem = JsonConvert.SerializeObject(data);

            //and set it to cache
            await _db.StringSetAsync(key, serializedItem, expiresIn);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        protected virtual async Task<bool> IsSetAsync(string key)
        {
            //little performance workaround here:
            //we use "PerRequestCacheManager" to cache a loaded object in memory for the current HTTP request.
            //this way we won't connect to Redis server many times per HTTP request (e.g. each time to load a locale or setting)
            //if (_perRequestCacheManager.IsSet(key))
            //    return true;

            return await _db.KeyExistsAsync(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        protected virtual async Task RemoveAsync(string key)
        {
            //we should always persist the data protection key list
            //if (key.Equals(RedisConfiguration.DataProtectionKeysName, StringComparison.OrdinalIgnoreCase))
            //    return;

            //remove item from caches
            await _db.KeyDeleteAsync(key);
            //_perRequestCacheManager.Remove(key);
        }

        /// <summary>
        /// Removes items by key pattern
        /// </summary>
        /// <param name="pattern">String key pattern</param>
        protected virtual async Task RemoveByPatternAsync(string pattern)
        {
            //_perRequestCacheManager.RemoveByPattern(pattern);

            foreach (var endPoint in _redisManager.muxer.GetEndPoints())
            {
                var server = _redisManager.muxer.GetServer(endPoint);
                var keys = server.Keys(database: _db.Database, pattern: $"*{pattern}*");

                //we should always persist the data protection key list
                //keys = keys.Where(key => !key.ToString().Equals(RedisConfiguration.DataProtectionKeysName, StringComparison.OrdinalIgnoreCase));

                await _db.KeyDeleteAsync(keys.ToArray());
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        protected virtual async Task ClearAsync()
        {
            //_perRequestCacheManager.Clear();

            foreach (var endPoint in _redisManager.muxer.GetEndPoints())
            {
                var server = _redisManager.muxer.GetServer(endPoint);

                //we can use the code below (commented), but it requires administration permission - ",allowAdmin=true"
                //server.FlushDatabase();

                //that's why we manually delete all elements
                var keys = server.Keys(database: _db.Database);

                //we should always persist the data protection key list
                //keys = keys.Where(key => !key.ToString().Equals(RedisConfiguration.DataProtectionKeysName, StringComparison.OrdinalIgnoreCase));

                await _db.KeyDeleteAsync(keys.ToArray());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Key of cached item</param>
        /// <returns>The cached value associated with the specified key</returns>
        public virtual T Get<T>(string key)
        {
            return this.GetAsync<T>(key).Result;
        }

        /// <summary>
        /// Adds the specified key and object to the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <param name="data">Value for caching</param>
        /// <param name="cacheTime">Cache time in minutes</param>
        public virtual async void Set(string key, object data, int cacheTime)
        {
            await this.SetAsync(key, data, cacheTime);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">Key of cached item</param>
        /// <returns>True if item already is in cache; otherwise false</returns>
        public virtual bool IsSet(string key)
        {
            return this.IsSetAsync(key).Result;
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">Key of cached item</param>
        public virtual async void Remove(string key)
        {
            await this.RemoveAsync(key);
        }

        /// <summary>
        /// Removes items by key pattern
        /// </summary>
        /// <param name="pattern">String key pattern</param>
        public virtual async void RemoveByPattern(string pattern)
        {
            await this.RemoveByPatternAsync(pattern);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual async void Clear()
        {
            await this.ClearAsync();
        }

        /// <summary>
        /// Dispose cache manager
        /// </summary>
        public virtual void Dispose()
        {
            //if (_connectionWrapper != null)
            //    _connectionWrapper.Dispose();
        }

        #endregion
    }
}
