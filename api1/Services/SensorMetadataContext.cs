using Microsoft.EntityFrameworkCore; 
using api1.Models;

namespace api1.Services {     
public class SensorMetadataContext : DbContext     
  {         
    public SensorMetadataContext(DbContextOptions<SensorMetadataContext> options): base(options){}       
    public DbSet<SensorMetadata> SensorMetadata { get; set; }     
  
   } 
}