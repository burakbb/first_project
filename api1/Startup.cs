using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using api1.Services;

namespace api1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<SensorMetadataContext>(options =>options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["RedisCache:ConnectionString"];
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("api1_swagger", new OpenApiInfo { Version = "1.0", Description = "Swagger for api1" });
            });
            services.AddHostedService<SensorDataConsumerService>();
            services.AddSingleton<ISensorDataService, SensorDataService>();
            services.AddSingleton<ICacheService,CacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            app.UseSwagger().UseSwaggerUI(option=>
            {
                option.SwaggerEndpoint("/swagger/api1_swagger/swagger.json","Swagger for api1");

            });
        }
    }
}
