using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using api1.Models;

using System;

namespace api1.Services{
    class SensorDataConsumerService : BackgroundService{
        private IConnection _connection;
        private IModel _sensorDataChannel;
        private IModel _reportDataChannel;
        private string _sensorDataQueueName;
        private string _reportDataQueueName;

        private ISensorDataService _sensorDataService;
        private readonly ICacheService _cacheService;


        public SensorDataConsumerService(IConfiguration configuration, ISensorDataService sensorDataService,ICacheService cacheService){
            var factory = new ConnectionFactory
            {
                HostName = configuration["rabbitmq:hostname"],
                UserName = configuration["rabbitmq:username"],
                Password = configuration["rabbitmq:password"],
            };
            _sensorDataQueueName = configuration["rabbitmq:sensorDataQueueChannel"];
            _reportDataQueueName = configuration["rabbitmq:reportDataQueueChannel"];

            _connection = factory.CreateConnection();
            _sensorDataChannel = _connection.CreateModel();
            _sensorDataChannel.QueueDeclare(queue: _sensorDataQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _reportDataChannel = _connection.CreateModel();


            _sensorDataService = sensorDataService;
            _cacheService = cacheService;
    } 
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var consumer = new EventingBasicConsumer(_sensorDataChannel);
                consumer.Received += (ch, ea) =>{
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());                
                    var sensorData = JsonSerializer.Deserialize<SensorData>(content);
                    Console.WriteLine(sensorData.Id.ToString()+" ,"+sensorData.Timestamp.ToString()+" ,"+ sensorData.Value.ToString());                
                    _sensorDataChannel.BasicAck(ea.DeliveryTag, false);
                    _sensorDataService.Insert(sensorData);
                    var lastCacheItem = _cacheService.GetSensorData(sensorData.Id);
                    if(lastCacheItem !=null){
                        _cacheService.SetSensorData(sensorData,lastCacheItem.sensorMetadata);
                        if(lastCacheItem.HasSensorData()){
                            var previousSensorData = lastCacheItem.sensorData;
                            if (previousSensorData.Value != sensorData.Value){
                                ReportData reportData = new ReportData{
                                    Id = sensorData.Id,
                                    StartDate = previousSensorData.Timestamp,
                                    EndDate = sensorData.Timestamp,
                                    Duration = (long)(sensorData.Timestamp - previousSensorData.Timestamp).TotalMilliseconds
                                };
                                _reportDataChannel.BasicPublish(exchange: "", routingKey: _reportDataQueueName, basicProperties: null, body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reportData)));
                                }                                
                            }
                        }
                    
                };
                _sensorDataChannel.BasicConsume(_sensorDataQueueName, false, consumer);
                await Task.CompletedTask;
            }
    }
}