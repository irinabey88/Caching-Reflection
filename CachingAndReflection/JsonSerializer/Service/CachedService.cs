using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using JsonSerializer.Interfaces;
using JsonSerializer.Models;
using StackExchange.Redis;

namespace JsonSerializer.Service
{
    public class CachedService : ICacheService
    {
        private readonly IJsonSerializer<TestObject> _jsonSerializer;
        private readonly IDatabase _db;
        private readonly IDictionary<int, TestObject> _localDb;

        public CachedService()
        {
            _localDb = new ConcurrentDictionary<int, TestObject>();
            _jsonSerializer = new JsonSerializer<TestObject>();

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            _db = redis.GetDatabase();
        }

        public void SaveData(TestObject testObject)
        {
            if (testObject == null)
            {
                throw new ArgumentNullException(nameof(testObject));
            }

            try
            {
                if (_localDb.ContainsKey(testObject.Id))
                {
                    throw new ArgumentException($"Element with such key exists!");
                }

                _localDb.Add(testObject.Id, testObject);
                _db.StringSet($"{testObject.Id}", _jsonSerializer.Serialize(testObject));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public TestObject GetData(int id)
        {
            try
            {
                var valueStr = _db.StringGet($"{id}").ToString();
                if (!string.IsNullOrWhiteSpace(valueStr))
                {
                    var valueObject = _jsonSerializer.Deserialize(valueStr);

                    //The expiration time was not exceeded
                    if (valueObject.Object != null)
                    {
                        return valueObject.Object;
                    }
                }

                var result = _localDb[id] ?? throw new ArgumentException($"Invalid id value!");
                _db.StringSet($"{result.Id}", _jsonSerializer.Serialize(result));

                Thread.Sleep(4000);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}