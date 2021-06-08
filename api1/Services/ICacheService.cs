using System;
using api1.Models;

namespace api1.Services{

    public interface ICacheService{
        CacheItem GetSensorData(Guid id);
        void SetSensorData(SensorData sensorData, SensorMetadata sensorMetadata);
        void RemoveSensorData(Guid id);
    }
}