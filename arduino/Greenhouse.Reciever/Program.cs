using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Greenhouse.Reciever
{
    class Program
    {
        static SerialPort sp = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);
        private static string outputfile = @"c:\temp\greenhouse\test.json";

        static void Main(string[] args)
        {
            sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
            sp.Open();
            Console.Read();

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
            foreach(var metric in metricsArray)
            {
                var metricInformation = metric.Split(' ');
                if(metricInformation.Length != 3)
                {
                    continue;
                }
                metrics.Add(new Metric { Timestamp =DateTime.Now, Name= metricInformation[0], Value=metricInformation[1], Unit=metricInformation[2] });
            }
            if (metrics.Count > 0) 
            { 
                string jsonString = JsonSerializer.Serialize(metrics);

                if (!File.Exists(outputfile))
                {
                    using (StreamWriter sw = File.CreateText(outputfile))
                    {
                        sw.WriteLine(jsonString);
                    }
                }
                using (StreamWriter sw = File.AppendText(outputfile))
                {
                    sw.WriteLine(jsonString);
                }
            }
        }
    }
}
