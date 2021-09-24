using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Greenhouse.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Greenhouse.Server.Watchers
{
    public class MetricsWatcher : BackgroundService
    {
        private readonly IHubContext<MetricsHub, IMetricsClient> _hubContext;

        private Timer _timer;

        public MetricsWatcher(IHubContext<MetricsHub, IMetricsClient> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(CheckMetrics, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }
        private async void CheckMetrics(object? state)
        {
            List<Metric> metrics = new List<Metric>();
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Humidity", Value = GetRandomNumber(20.00, 54.00).ToString(), Unit = "%" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Temperature-Air", Value = GetRandomNumber(20.00, 30.00).ToString(), Unit = "C" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Temperature-Earth", Value = GetRandomNumber(20.00, 25.00).ToString(), Unit = "C" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Light", Value = GetRandomNumber(25.00, 700.00).ToString(), Unit = "Lumen" });
            await _hubContext.Clients.All.ReceiveMetrics(metrics);
        }
        
        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}