using System;
using System.ComponentModel;
using System.Net.Http;
using System.Windows.Input;
using DIPS.Xamarin.UI.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Greenhouse.Mobile
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private bool _isTestingServerConnection;
        private string _testingServerConnectionErrorMessage;
        private bool _canConnectToServer;

        public MainViewModel()
        {
            TestServerConnectionCommand = new Command(TestServerConnection);
            var httpHandler = new HttpClientHandler();
            //Skip server cert, who cares?
            httpHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
            _httpClient = new HttpClient(httpHandler);
        }

        public string ServerAddress
        {
            get => Preferences.Get("ServerAddress", "");
            set
            {
                Preferences.Set("ServerAddress",value);
                PropertyChanged.Raise();
            }
        }

        public string TestingServerConnectionErrorMessage
        {
            get => _testingServerConnectionErrorMessage;
            set => PropertyChanged.RaiseWhenSet(ref _testingServerConnectionErrorMessage, value);
        }

        public bool IsTestingServerConnection
        {
            get => _isTestingServerConnection;
            set => PropertyChanged.RaiseWhenSet(ref _isTestingServerConnection, value);
        }

        public ICommand TestServerConnectionCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        private async void TestServerConnection()
        {
            try
            {
                IsTestingServerConnection = true;
                CanConnectToServer = false;
                TestingServerConnectionErrorMessage = null;
                var response = await _httpClient.GetAsync(ServerAddress + "/status/ping");
                if (response.IsSuccessStatusCode)
                {
                    CanConnectToServer = true;
                }
                else
                {
                    TestingServerConnectionErrorMessage = response.ReasonPhrase;
                }
            }
            catch (Exception exception)
            {
                TestingServerConnectionErrorMessage = exception.Message;
            }
            finally
            {
                IsTestingServerConnection = false;
            }
        }

        public bool CanConnectToServer
        {
            get => _canConnectToServer;
            set => PropertyChanged.RaiseWhenSet(ref _canConnectToServer, value);
        }
    }
}