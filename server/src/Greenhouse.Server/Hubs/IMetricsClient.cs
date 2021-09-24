using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greenhouse.Server.Hubs
{
    public interface IMetricsClient
    {
        /// <summary>
        /// Used to push a new message about new metrics
        /// </summary>
        /// <param name="data">The metrics data</param>
        /// <returns></returns>
        Task ReceiveMetrics(List<Metric> data);
    }
}