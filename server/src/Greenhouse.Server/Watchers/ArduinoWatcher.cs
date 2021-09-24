
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace Greenhouse.Server
{
    public class ArduinoWatcher
    {
        private static Timer m_timer;
        static SerialPort sp = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
        private static List<Metric> m_latestMetrics;
        private static Action<List<Metric>> _metricsChangedAction;
        private static bool IsLoaded = false;

        public static void Initialize(Action<List<Metric>> metricsChangedAction)
        {
            _metricsChangedAction = metricsChangedAction;
            if (!IsLoaded)
            {
                try
                {
                    m_latestMetrics = new List<Metric>();
                    sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
                    sp.Open();
                }
                catch (Exception)
                {
                    m_timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

                }
                IsLoaded = true;
            }
        }

        private static void DoWork(object state)
        {
            //;Humidity 54.00 %;Temperature-Air 23.00 C;Temperature-Earth 20.50 C;Light 12.60 Lumen;Soil 1029.31 unknown;;

            List<Metric> metrics = new List<Metric>();
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Humidity-Air", Value = GetRandomNumber(20.00, 54.00).ToString(), Unit = "%" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Humidity-Earth", Value = GetRandomNumber(20.00, 54.00).ToString(), Unit = "%" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Temperature-Air", Value = GetRandomNumber(20.00, 30.00).ToString(), Unit = "C" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Temperature-Earth", Value = GetRandomNumber(20.00, 25.00).ToString(), Unit = "C" });
            metrics.Add(new Metric { Timestamp = DateTime.Now, Name = "Light", Value = GetRandomNumber(25.00, 700.00).ToString(), Unit = "Lumen" });
            LatestMetrics = metrics;
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }


        private static void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            List<Metric> metrics = new List<Metric>();
            var value = sp.ReadLine();
            var metricsArray = value.Split(';');
            if (metricsArray.Length == 0)
            {
                return;
            }
            foreach (var metric in metricsArray)
            {
                var metricInformation = metric.Split(' ');
                if (metricInformation.Length != 3)
                {
                    continue;
                }
                metrics.Add(new Metric { Timestamp = DateTime.Now, Name = metricInformation[0], Value = metricInformation[1], Unit = metricInformation[2] });
            }
            LatestMetrics = metrics;
        }

        public static List<Metric> LatestMetrics
        {
            private set
            {
                m_latestMetrics = value;
                _metricsChangedAction?.Invoke(value);
            }
            get { return m_latestMetrics; }
        }
    }
}