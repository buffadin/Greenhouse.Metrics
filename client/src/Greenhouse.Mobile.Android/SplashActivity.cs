using Android.App;
using Android.Content;
using AndroidX.AppCompat.App;

namespace Greenhouse.Mobile.Android
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof (MainActivity)));
        }
    }
}