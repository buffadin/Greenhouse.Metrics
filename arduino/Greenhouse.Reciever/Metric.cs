using System;
using System.Collections.Generic;
using System.Text;

namespace Greenhouse.Reciever
{
    public class Metric
    {
        public DateTime Timestamp { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
    }
}
