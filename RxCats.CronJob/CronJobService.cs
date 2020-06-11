using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace RxCats.CronJob
{
    public class CronJobService : IHostedService, IDisposable
    {
        private Timer timer;

        private readonly CronExpression expression;

        private readonly TimeZoneInfo timeZoneInfo;

        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo)
        {
            expression = CronExpression.Parse(cronExpression);
            this.timeZoneInfo = timeZoneInfo;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            return ScheduleJob(cancellationToken);
        }

        protected virtual Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = expression.GetNextOccurrence(DateTimeOffset.Now, timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                timer = new Timer(delay.TotalMilliseconds);
                timer.Elapsed += (sender, args) =>
                {
                    timer.Dispose(); // reset and dispose timer
                    timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        DoWork(cancellationToken);
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        ScheduleJob(cancellationToken); // reschedule next
                    }
                };
                timer.Start();
            }

            return Task.CompletedTask;
        }

        public virtual Task DoWork(CancellationToken cancellationToken)
        {
            return Task.Delay(5000, cancellationToken); // do the work
        }

        public virtual Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Stop();
            return Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            timer?.Dispose();
        }
    }

    public interface IScheduleConfig
    {
        string CronExpression { get; set; }

        TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public class ScheduleConfig : IScheduleConfig
    {
        public string CronExpression { get; set; }

        public TimeZoneInfo TimeZoneInfo { get; set; }
    }

    public static class ScheduledServiceExtensions
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }

            var config = new ScheduleConfig();
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(ScheduleConfig.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<IScheduleConfig>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}