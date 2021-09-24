using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        private MetricViewModel _currentMetric;
        private bool _isConnectedToServer;
        private bool _isConnectingToServer;
        private bool _isMetricDetailsOpen;
        private string _serverConnectionErrorMessage;

        public MainViewModel()
        {
            ConnectToServerCommand = new Command(ConnectToServer);
            OpenMetricDetailsCommand = new Command<MetricViewModel>(m =>
            {
                CurrentMetric = m;
                IsMetricDetailsOpen = true;
            });
            _httpClient = new HttpClient(new HttpClientHandler
                {ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true});
            Metrics = new List<MetricViewModel>
            {
                new MetricViewModel("Temperature-Air", MetricType.TemperatureAir, "temperature.png"),
                new MetricViewModel("Temperature-Earth", MetricType.TemperatureEarth, "temperature.png"),
                new MetricViewModel("Humidity-Air", MetricType.HumidityAir, "humidity.png"),
                new MetricViewModel("Humidity-Earth", MetricType.HumiditiyEarth, "humidity.png"),
                new MetricViewModel("Light", MetricType.Light, "light.png")
            };
        }

        public HubConnection HubConnection { get; set; }
        public List<MetricViewModel> Metrics { get; }

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

        public ICommand OpenMetricDetailsCommand { get; }

        public bool IsMetricDetailsOpen
        {
            get => _isMetricDetailsOpen;
            set => PropertyChanged.RaiseWhenSet(ref _isMetricDetailsOpen, value);
        }

        public MetricViewModel CurrentMetric
        {
            get => _currentMetric;
            set => PropertyChanged.RaiseWhenSet(ref _currentMetric, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ConnectToServer()
        {
            try
            {
                IsConnectingToServer = true;
                IsConnectedToServer = false;
                ServerConnectionErrorMessage = null;
                await _httpClient.GetAsync($"{ServerAddress}/status/ping");
                if (HubConnection != null) await HubConnection.StopAsync();
                HubConnection = new HubConnectionBuilder()
                    .WithUrl($"{ServerAddress}/MetricsHub",
                        options => options.HttpMessageHandlerFactory = x => new HttpClientHandler
                            {ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true})
                    .WithAutomaticReconnect()
                    .Build();


                ListenForHubChanges();

                await HubConnection.StartAsync();
                switch (HubConnection.State)
                {
                    case HubConnectionState.Disconnected:
                        break;
                    case HubConnectionState.Connected:
                        HubConnection.On<List<Metric>>("ReceiveMetrics", OnMetricsChanged);
                        IsConnectedToServer = true;
                        break;
                    case HubConnectionState.Connecting:
                        break;
                    case HubConnectionState.Reconnecting:
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
                PropertyChanged.Raise(nameof(HubConnection));
            }
        }

        private void ListenForHubChanges()
        {
            HubConnection.Closed += async exception =>
            {
                await HubConnection.StartAsync();
                PropertyChanged.Raise(nameof(HubConnection));
            };

            HubConnection.Reconnecting += exception =>
            {
                PropertyChanged.Raise(nameof(HubConnection));
                return Task.CompletedTask;
            };

            HubConnection.Reconnected += s =>
            {
                PropertyChanged.Raise(nameof(HubConnection));
                return Task.CompletedTask;
            };
        }

        private void OnMetricsChanged(List<Metric> metrics)
        {
            foreach (var metric in metrics)
            {
                var metricViewModel = Metrics.FirstOrDefault(m => m.Name == metric.Name);
                if (metricViewModel != null) metricViewModel.CurrentMetric = metric;
            }
        }

        public void Initialize()
        {
            ConnectToServer();
        }
    }
}