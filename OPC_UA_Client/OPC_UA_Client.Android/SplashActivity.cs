using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace OPC_UA_Client.Droid
{
  
        [Activity(Label = "OPC Client",  Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
        public class SplashActivity : AppCompatActivity
        {
            protected override void OnResume()
            {
                base.OnResume();
                StartActivity(typeof(MainActivity));
            }
        }
    
}