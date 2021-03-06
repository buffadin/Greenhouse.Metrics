using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Greenhouse.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        public ICommand OpenSettingsCommand { get; } = new Command(() => Application.Current.MainPage.Navigation.PushModalAsync(new SettingsPage()));
    }
}