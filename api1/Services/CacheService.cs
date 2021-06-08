using System;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

using api1.Models;

namespace api1.Services{

    class CacheService: ICacheService{
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache){
            _distributedCache = distributedCache;
        }

        public CacheItem GetSensorData(Guid id){
            try{
                var bytes = _distributedCache.Get(id.ToString());                
                if (bytes == null){
                    return null;
                }
                else{
                    var item = JsonSerializer.Deserialize<CacheItem>(bytes);
                    return item;
                }
            }
            catch(Exception e){
                Console.WriteLine(e);
                return null;
            }
        }
        public void SetSensorData(SensorData sensorData, SensorMetadata sensorMetadata){
            try{
                var cacheItem = new CacheItem { sensorMetadata= sensorMetadata, sensorData = sensorData};
                _distributedCache.Set(sensorMetadata.Id.ToString(),Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cacheItem)));
            }
            catch(Exception e){
                Console.WriteLine(e);
            }
        }
        public void RemoveSensorData(Guid id){
            try{
                _distributedCache.Remove(id.ToString());
            }
            catch(Exception e){
                Console.WriteLine(e);
            }
        }
    }
}