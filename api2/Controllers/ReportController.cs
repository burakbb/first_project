using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;

using api2.Models;


namespace api2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportDataController : ControllerBase
    {
        private readonly ElasticClient _elasticClient;
        public ReportDataController(ElasticClient elasticClient){
            _elasticClient = elasticClient;
            
        }

        [HttpPost]
        public ActionResult<ResultData> GetAverageDuration(DateTime StartDate, DateTime EndDate,Guid id){
            try{
                if( StartDate == null || EndDate == null){
                    return BadRequest();
                }
                if(id != null && !id.Equals(Guid.Empty)){
                    var searchResponse = _elasticClient.Search<object>(s => s
                        .Size(0)
                        .Aggregations(a => a
                            .Filter("id_match", f => f
                                .Filter(q => q.Term("sensorId",id)
                            )
                                .Aggregations(aa => aa
                                    .Average("avg_duration", ad => ad
                                        .Field("duration")
                                    )
                            )
                        )
                    )
                );
                        //Console.WriteLine(searchResponse.DebugInformation);
                        var average_duration = (double)searchResponse.Aggregations.Children("id_match").Average("avg_duration").Value.GetValueOrDefault();
                        //searchResponse.Aggregations.Filter("id_match").Ag
                        return new ResultData{
                            sensorId = id,
                            averageDuration = average_duration
                        };
                    }
                    else{
                        var searchResponse = _elasticClient.Search<object>(s => s
                             .Size(0)
                            .Aggregations(aa => aa
                                        .Average("avg_duration", ad => ad
                                            .Field("duration")
                            )));
                        
                        var average_duration = (double)searchResponse.Aggregations.Average("avg_duration").Value.GetValueOrDefault();
                        
                        return new ResultData{
                            sensorId = Guid.Empty,
                            averageDuration = average_duration
                        };

                    }
                }
                catch(Exception e){
                    return BadRequest();
                }
        }
    }
}