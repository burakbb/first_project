using System;

namespace api2.Models
{
    public class ReportData{
        public Guid Id { get; set; }//DocumentId
        public Guid SensorId { get; set; }//SensorID
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
        public long Duration { get; set; }// Change Duration	        
    }
}