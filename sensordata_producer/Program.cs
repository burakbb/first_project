using System;
using RabbitMQ.Client;
using System.Threading;
using System.Text;
using System.Text.Json;


namespace sensordata_producer
{
    class Program
    {
        static void Main(string[] args)
        {
                var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
                using (var connection = factory.CreateConnection()){
                    using (var channel = connection.CreateModel()){
                        long i=0;
                        string[] guids = { "3e491381-d795-489a-927c-6c9baa9582b1", "494be82f-ad85-4a03-bacd-bd1366685bc4",
                                    "f3df24ee-27a8-4400-a6f4-f3e0c247c1b1", "9c0d18f9-8739-4342-882e-e3dbb459240c",
                                    "b3d6384a-48ce-445a-b0c8-3109356bdfe5", "625da4ac-09ca-4992-9ef2-2b78afabda02",
                                    "8720a225-8bf5-4a27-8f0a-e3cd04f146d0", "0a9662bd-c413-4de1-81a9-a93bf756e0a9",
                                    "a6c979ec-254a-403a-8a11-1786a6e03a49", "104fc540-9702-4b54-9043-21298d8c43b5",
                                    "6036b49b-69d3-4fdd-bb96-a5fc524bc86d"
                                    };
                        Random rnd = new Random();
                        while(true){
                            i++;
                            var randomNumber = rnd.Next(100);
                            var randomGuid = new Guid(guids[randomNumber%11]);
                            SensorData data = new SensorData{
                                Id= randomGuid,
                                Timestamp= DateTime.Now,
                                Value= randomNumber < 50 ? true : false
                                
                            };
                            
                            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
                            channel.BasicPublish(exchange: "", routingKey: "sensor_data", basicProperties: null, body: body);
                            Thread.Sleep(100);
                        }

                    }
                }
      
        }
    }
}
