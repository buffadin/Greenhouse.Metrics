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
        private bool _isTestingServerAddress;
        private string _testingServerAddressErrorMessage;
        private bool _canConnectToServer;

        public MainViewModel()
        {
            ServerAddressAddedCommand = new Command(ServerAddressAdded);
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

        public string TestingServerAddressErrorMessage
        {
            get => _testingServerAddressErrorMessage;
            set => PropertyChanged.RaiseWhenSet(ref _testingServerAddressErrorMessage, value);
        }

        public bool IsTestingServerAddress
        {
            get => _isTestingServerAddress;
            set => PropertyChanged.RaiseWhenSet(ref _isTestingServerAddress, value);
        }

        public ICommand ServerAddressAddedCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        private async void ServerAddressAdded()
        {
            try
            {
                IsTestingServerAddress = true;
                CanConnectToServer = false;
                TestingServerAddressErrorMessage = null;
                var response = await _httpClient.GetAsync(ServerAddress + "/status/ping");
                if (response.IsSuccessStatusCode)
                {
                    CanConnectToServer = true;
                }
                else
                {
                    TestingServerAddressErrorMessage = response.ReasonPhrase;
                }
            }
            catch (Exception exception)
            {
                TestingServerAddressErrorMessage = exception.Message;
            }
            finally
            {
                IsTestingServerAddress = false;
            }
        }

        public bool CanConnectToServer
        {
            get => _canConnectToServer;
            set => PropertyChanged.RaiseWhenSet(ref _canConnectToServer, value);
        }
    }
}