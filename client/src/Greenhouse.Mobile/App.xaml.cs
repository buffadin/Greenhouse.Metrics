using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Greenhouse.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
            
            var resource = Resources["MainViewModel"];
            if (resource is MainViewModel mainViewModel)
            {
                MainViewModel = mainViewModel;
            }
        }

        public MainViewModel MainViewModel { get; }

        protected override void OnStart()
        {
            MainViewModel.Initialize();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}