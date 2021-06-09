using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Nest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System;

using api2.Models;

namespace api2.Services{
    class ReportDataConsumerService:BackgroundService{
        private IConnection _connection;
        private IModel _reportDataChannel;
        private string _reportDataQueueName;
        private readonly ElasticClient _elasticClient;

        public ReportDataConsumerService(ElasticClient elasticClient, IConfiguration configuration){
            var factory = new ConnectionFactory{
                HostName = configuration["rabbitmq:hostname"],
                UserName = configuration["rabbitmq:username"],
                Password = configuration["rabbitmq:password"],
            };
            _reportDataQueueName = configuration["rabbitmq:channel"];
            _connection = factory.CreateConnection();
            _reportDataChannel = _connection.CreateModel();
            _elasticClient = elasticClient;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken){
            cancellationToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_reportDataChannel);
            consumer.Received += (ch, ea) => {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var reportData = JsonSerializer.Deserialize<ReportData>(content);
                reportData.SensorId = reportData.Id;
                reportData.Id = Guid.NewGuid();
                var response =_elasticClient.IndexDocument<ReportData>(reportData);
                _reportDataChannel.BasicAck(ea.DeliveryTag,false);
                
            };
            _reportDataChannel.BasicConsume(_reportDataQueueName, false, consumer);
            await Task.CompletedTask;
        }
    }
}