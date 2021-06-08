using System;

namespace api1.Models
{
        public class CacheItem
    {
            public SensorMetadata sensorMetadata {get;set;}
            public SensorData sensorData {get;set;}

            public bool HasSensorData(){
                    return this.sensorData.Id != Guid.Empty;
            }
    }
}
