using System;

namespace Greenhouse.Mobile.Metrics
{
    public class Metric
    {
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}