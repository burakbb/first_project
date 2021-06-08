using System;

namespace api1.Models
{
        public class SensorData
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Value { get; set; }	
    }
}
