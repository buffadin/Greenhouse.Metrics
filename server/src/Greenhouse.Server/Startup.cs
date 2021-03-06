using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greenhouse.Server.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Greenhouse.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // For mobile apps, allow http traffic.
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MetricsHub>("/metricsHub");
                endpoints.MapGet("/status/ping", async context => { await context.Response.WriteAsync("pong"); });
            });
            
            app.Use(async (context, func) =>
            {
                var metricsHub = context.RequestServices
                    .GetRequiredService<IHubContext<MetricsHub, IMetricsClient>>();
                ArduinoWatcher.Initialize(metrics =>
                    {
                        metricsHub.Clients.All.ReceiveMetrics(metrics);
                    }
                );
            });
        }
    }
}