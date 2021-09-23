using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Greenhouse.Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            DIPS.Xamarin.UI.Library.Initialize();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (SplashView.IsVisible)
            {
                await SplashView.Splash();
                await SplashView.FadeTo(0,750);
                SplashView.IsVisible = false;
            }
        }
    }
}