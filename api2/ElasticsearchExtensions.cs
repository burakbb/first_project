using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

using api2.Models;

namespace api2{
    public static class ElasticsearchExtensions{
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration){
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];
            var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(defaultIndex);
            var client = new ElasticClient(settings);            
            services.AddSingleton(client);
            CreateIndex(client, defaultIndex);
        }

        public static void CreateIndex(IElasticClient client, string indexName){
            var createIndexResponse = client.Indices.Create(indexName, index =>
                index.Map<ReportData>(x=> x.AutoMap()));
        }
    }
}