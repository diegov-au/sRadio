using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace sRadio
{
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.OS;
    using Android.Support.V7.App;
    using Android.Util;

    namespace com.xamarin.sample.splashscreen
    {
        [Activity(Theme = "@style/SplashTheme.Splash", MainLauncher = true, NoHistory = true)]
        public class SplashActivity : AppCompatActivity
        {
            static readonly string TAG = "X:" + typeof(SplashActivity).Name;

            public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
            {
                base.OnCreate(savedInstanceState, persistentState);
              
            }

            // Launches the startup task
            protected override void OnResume()
            {
                base.OnResume();
              
                Task startupWork = new Task(() => { Startup(); });
                startupWork.Start();
            }

            // Prevent the back button from canceling the startup process
            public override void OnBackPressed() { }

            // Simulates background work that happens behind the splash screen
            void Startup()
            {
               
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.NoAnimation);
                StartActivity(intent);
                OverridePendingTransition(0, 0);
                Finish();

                //StartActivity(new Intent(Application.Context, typeof(MainActivity)));

            }
        }
    }
}