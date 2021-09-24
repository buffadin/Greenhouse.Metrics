using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DIPS.Xamarin.UI.Extensions;
using Greenhouse.Mobile.Metrics;
using Microsoft.AspNetCore.SignalR.Client;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Greenhouse.Mobile
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private bool _isConnectedToServer;
        private bool _isConnectingToServer;
        private string _serverConnectionErrorMessage;
        private HubConnection _singleRHubConnection;

        public List<MetricViewModel> Metrics { get; }

        public MainViewModel()
        {
            ConnectToServerCommand = new Command(ConnectToServer);
            _httpClient = new HttpClient(new HttpClientHandler
                {ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true});
            Metrics = new List<MetricViewModel>()
            {
                new MetricViewModel("Temperature-Air", MetricType.TemperatureAir, "temperature.png"),
                new MetricViewModel("Temperature-Earth", MetricType.TemperatureEarth, "temperature.png"),
                new MetricViewModel("Humidity", MetricType.Humidity, "humidity.png"),
                new MetricViewModel("Light", MetricType.Light, "light.png"),
            };
        }

        public string ServerAddress
        {
            get => Preferences.Get("ServerAddress", "");
            set
            {
                Preferences.Set("ServerAddress", value);
                PropertyChanged.Raise();
            }
        }

        public string ServerConnectionErrorMessage
        {
            get => _serverConnectionErrorMessage;
            set => PropertyChanged.RaiseWhenSet(ref _serverConnectionErrorMessage, value);
        }

        public bool IsConnectingToServer
        {
            get => _isConnectingToServer;
            set => PropertyChanged.RaiseWhenSet(ref _isConnectingToServer, value);
        }

        public ICommand ConnectToServerCommand { get; }

        public bool IsConnectedToServer
        {
            get => _isConnectedToServer;
            set => PropertyChanged.RaiseWhenSet(ref _isConnectedToServer, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ConnectToServer()
        {
            try
            {
                IsConnectingToServer = true;
                IsConnectedToServer = false;
                ServerConnectionErrorMessage = null;
                var response = await _httpClient.GetAsync($"{ServerAddress}/status/ping");
                if (_singleRHubConnection != null) await _singleRHubConnection.StopAsync();
                _singleRHubConnection = new HubConnectionBuilder()
                    .WithUrl($"{ServerAddress}/MetricsHub",
                        options => options.HttpMessageHandlerFactory = x => new HttpClientHandler
                            {ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true})
                    .Build();
                await _singleRHubConnection.StartAsync();
                switch (response.IsSuccessStatusCode)
                {
                    case true when _singleRHubConnection.State == HubConnectionState.Connected:
                        _singleRHubConnection.On<List<Metric>>("ReceiveMetrics", OnMetricsChanged);
                        IsConnectedToServer = true;
                        break;
                    case false:
                        ServerConnectionErrorMessage = response.ReasonPhrase;
                        break;
                }
            }
            catch (Exception exception)
            {
                ServerConnectionErrorMessage = exception.Message;
            }
            finally
            {
                IsConnectingToServer = false;
            }
        }

        private void OnMetricsChanged(List<Metric> metrics)
        {
            foreach (var metric in metrics)
            {
                var metricViewModel = Metrics.FirstOrDefault(m => m.Name == metric.Name);
                if (metricViewModel != null)
                {
                    metricViewModel.Metric = metric;
                }
            }
        }

        public void Initialize()
        {
            ConnectToServer();
        }
    }

    public enum MetricType
    {
        TemperatureAir,
        TemperatureEarth,
        Humidity,
        Light
    }

    public class MetricViewModel : INotifyPropertyChanged
    {
        private Metric _metric;
        public string Name { get; }
        public MetricType MetricType { get; }
        public string IconName { get; }

        public MetricViewModel(string name, MetricType metricType, string iconName)
        {
            Name = name;
            MetricType = metricType;
            IconName = iconName;
        }

        public Metric Metric
        {
            get => _metric;
            set => PropertyChanged.RaiseWhenSet(ref _metric, value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }
}