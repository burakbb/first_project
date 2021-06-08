using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;


using api1.Models;
using api1.Services;

namespace api1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorMetadataController : ControllerBase
    {
        private readonly SensorMetadataContext _context;
        private readonly ICacheService _cacheService;
        public SensorMetadataController(SensorMetadataContext context,ICacheService cacheService){
            _context = context;
            _cacheService = cacheService;
        }
        [HttpPost()]
        public ActionResult Create([FromBody] SensorMetadata sensorMetadata)
        {
            try{
                _context.SensorMetadata.Add(sensorMetadata);
                int rowChanged = _context.SaveChanges();
                if ( rowChanged == 1){
                    _cacheService.SetSensorData(new SensorData(),sensorMetadata);
                    return Ok();
                }
                else {
                    return BadRequest();    
                    }
            }
            catch{
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<SensorMetadata> Read(Guid id)
        {
            try{
                var cacheItem = _cacheService.GetSensorData(id);                
                if (cacheItem == null){
                    return NotFound();
                }
                else{
                    return cacheItem.sensorMetadata;
                }
            }
            catch{
                return BadRequest();
            }
        }

        [HttpPut()]
        public ActionResult Update([FromBody] SensorMetadata sensorMetadata)
        {
            try{
                var item = _context.SensorMetadata.Find(sensorMetadata.Id);
                if (item == null){
                    return NotFound();
                }
                else{
                    item.Name = sensorMetadata.Name;
                    int rowsUpdated =_context.SaveChanges();
                    if (rowsUpdated == 1){
                        _cacheService.SetSensorData(new SensorData(),sensorMetadata);
                        return Ok();
                    }
                    else{
                        return NotFound();
                    }
                }
            }
            catch{
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete( Guid id)
        {
            try{
                var item = _context.SensorMetadata.Find(id);
                if (item == null){
                    return NotFound();
                }
                else{
                    _context.Remove(item);
                    int rowsUpdated =_context.SaveChanges();
                    if (rowsUpdated == 1){
                        _cacheService.RemoveSensorData(item.Id);
                        return Ok();
                    }
                    else{
                        return NotFound();
                    }
                }
            }
            catch{
                return BadRequest();
            }

        }        
    }
}
