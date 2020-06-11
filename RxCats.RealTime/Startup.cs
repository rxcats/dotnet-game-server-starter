using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RxCats.CronJob;
using RxCats.RealTime.Jobs;
using RxCats.WebSocketExtensions;

namespace RxCats.RealTime
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

            services.AddSingleton<TextWebSocketHandler>();
            services.AddSingleton<WebSocketEventHandler>();
            services.AddSingleton<WebSocketSessionFactory>();
            services.AddSingleton<GameSlotFactory>();

            // cron jobs
            services.AddCronJob<PingJob>(config =>
            {
                config.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                config.CronExpression = @"*/1 * * * *";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4096
            });

            app.UseWebSocketMiddleware();
        }
    }
}