using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ScheduleMessages.Infrastructure.EventHandling;

namespace ScheduleMessages
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScheduleMessages", Version = "v1" });
            });
            
            services.AddOptions();
            services.AddHealthChecks();
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("ready");
            });
            var region = Configuration.GetValue("AmazonEventBusSettings:Region", "eu-west-1");
            var ACCESS_KEY_ID = Configuration.GetValue<string>("AWS_ACCESS_KEY_ID");
            var SECRET_ACCESS_KEY = Configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY");
             services.AddMassTransit(x =>
            {
                
                x.AddDelayedMessageScheduler();
                // add all consumers in the specified assembly
                x.AddConsumer(typeof(ScheduleNotificationConsumer));
                x.AddConsumer(typeof(SendNotificationConsumer));
                
                x.UsingAmazonSqs((context, cfg) =>
                {

                    cfg.Host(region, h =>
                    {
                       
                        
                        h.AccessKey(ACCESS_KEY_ID);
                        h.SecretKey(SECRET_ACCESS_KEY);
                        
                        // scope topics as well
                        h.EnableScopedTopics();


                    });
                    cfg.UseDelayedMessageScheduler();
                    cfg.ConfigureEndpoints(context);
                    
                });

            });

            services.AddMassTransitHostedService();
           
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build());
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
                
                endpoints.MapHealthChecks( "/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                });
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {

                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
            });
        }
    }
}
