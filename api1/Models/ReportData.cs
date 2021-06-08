using System;

namespace api1.Models
{
    class ReportData{
        public Guid Id { get; set; }//SensorID
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public long Duration { get; set; }// Change Duration	        
    }
}