using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Greenhouse.Mobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashView
    {
        public SplashView()
        {
            InitializeComponent();
        }
        

        public Task Splash()
        {
            return Text.FadeTo(1,1000);
        }
    }
}