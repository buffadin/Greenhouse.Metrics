using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Greenhouse.Server.Hubs
{
    public class MetricsHub : Hub<IMetricsClient>
    {
        //Use this SingleR message to send a message about metrics changed
        public async Task MectricsChanged(string data)
        {
            await Clients.All.ReceiveMetrics(data);
            Console.WriteLine("Metrics changed from SingleR hub");
        }
    }
}