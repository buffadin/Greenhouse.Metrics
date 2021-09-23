using System;
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

        public MetricsWatcher(IHubContext<MetricsHub, IMetricsClient> hubContext)
        {
            _hubContext = hubContext;
        }
        
        private Timer _timer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(CheckMetrics, null, TimeSpan.Zero, 
                TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private async void CheckMetrics(object? state)
        {
            Console.WriteLine("Fetching metrics");
            await _hubContext.Clients.All.ReceiveMetrics("SomeData");
        }
    }
}