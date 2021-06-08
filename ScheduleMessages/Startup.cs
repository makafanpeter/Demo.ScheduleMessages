using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using MassTransit;

namespace ScheduleMessages
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScheduleMessages", Version = "v1" });
            });
            
            
            var region = Configuration.GetValue("AmazonEventBusSettings:Region", "eu-west-1");
             services.AddMassTransit(x =>
            {
                // add all consumers in the specified assembly
                x.AddConsumers();

                x.AddDelayedMessageScheduler();

                x.UsingAmazonSqs((context, cfg) =>
                {
                    
                    
                    cfg.Host(region, h =>
                    {
                       
                        h.Credentials(new EnvironmentVariablesAWSCredentials());

                        


                        // scope topics as well
                        h.EnableScopedTopics();
                        
                        cfg.UseDelayedMessageScheduler();
                    });
                    


                });

            });

            services.AddMassTransitHostedService();
           
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScheduleMessages v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
