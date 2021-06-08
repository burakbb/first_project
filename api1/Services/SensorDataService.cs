using Cassandra;
using Microsoft.Extensions.Caching.Distributed;
using api1.Models;

namespace api1.Services{
    class SensorDataService: ISensorDataService{
        private readonly Cluster _cluster;
        private readonly ISession _session;
        private readonly ICacheService _cacheService;
        public SensorDataService(ICacheService cacheService){
            _cluster = Cluster.Builder().AddContactPoint("localhost").Build();
            _session = _cluster.Connect("sensordata_db");
            _cacheService = cacheService;
        }

        public void Insert(SensorData sensorData ){
            var ps = _session.Prepare("INSERT INTO sensor_data (id, timestamp, value) values(?, ?, ?)");
            var statement = ps.Bind(sensorData.Id, sensorData.Timestamp, sensorData.Value);
            var result = _session.Execute(statement);    
        }
    }
}