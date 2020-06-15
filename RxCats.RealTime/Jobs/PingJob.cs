using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RxCats.CronJob;
using RxCats.WebSocketExtensions;

namespace RxCats.RealTime.Jobs
{
    public class PingJob : CronJobService
    {
        private readonly ILogger<PingJob> logger;

        private readonly WebSocketSessionManager webSocketSessionManager;

        public PingJob(IScheduleConfig config, ILogger<PingJob> logger, WebSocketSessionManager webSocketSessionManager)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            this.logger = logger;
            this.webSocketSessionManager = webSocketSessionManager;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("PingJob 1 starts.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{DateTime.Now:hh:mm:ss} PingJob 1 is working.");

            var closedSessions = new List<long>();
            var sessions = webSocketSessionManager.All();
            foreach (var session in sessions)
            {
                if (!session.Value.IsOpen())
                {
                    closedSessions.Add(session.Key);
                    continue;
                }
                session.Value.SendAsyncPing();
            }

            foreach (var session in closedSessions)
            {
                webSocketSessionManager.RemoveByKey(session);
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("PingJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}