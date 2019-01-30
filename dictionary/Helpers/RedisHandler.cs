﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
namespace dictionary.Helpers
{
    public class RedisHandler
    {
        private readonly ConnectionMultiplexer Connection;
        private readonly IDatabaseAsync db;
        public RedisHandler()
        {
            Connection = ConnectionMultiplexer.Connect("localhost:6379");
            db = Connection.GetDatabase(1);
        }

        public async Task<string> GetFromCache(string key)
        {
            var isCached = await IsCached(key);
            if (!isCached)
            {
                return null;
            }
            var cachedData = await db.StringGetAsync(key);
            return await Task.FromResult(cachedData);

        }

        public async Task<bool> AddToCache(string key, TimeSpan timeout, string data)
        {
            var isCached = await IsCached(key);
            if (isCached)
            {
                return await Task.FromResult(false);
            }

            await db.StringSetAsync(key, data, timeout);
            return await Task.FromResult(true);
        }

        public async Task<bool> IsCached(string key)
        {
            var cachedData = await db.StringGetAsync(key);
            if (string.IsNullOrEmpty(cachedData))
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);

        }
    }
}
