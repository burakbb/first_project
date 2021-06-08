using System;

namespace sensordata_producer
{
        public class SensorData
    {
        public Guid Id { get; set; }//SensorID
        public DateTime Timestamp { get; set; }
        public bool Value { get; set; }	
    }
}
