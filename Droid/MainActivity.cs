using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Net;
using Serilog;
namespace qController.Droid
{
    [Activity(Label = "qController", Icon = "@drawable/icon", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Log.Logger = new LoggerConfiguration().WriteTo.AndroidLog().CreateLogger();

            LoadApplication(new App());

            Acr.UserDialogs.UserDialogs.Init(this);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                ConnectivityManager connMgr = (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
                Network[] networks = connMgr.GetAllNetworks();
                for (int i = 0; i < networks.Length; i++)
                {
                    Network network = networks[i];
                    NetworkInfo networkInfo = connMgr.GetNetworkInfo(network);
                    if (networkInfo.Type == ConnectivityType.Wifi && networkInfo.IsAvailable && networkInfo.IsConnected)
                    {
                        Log.Debug("ANDROID MAIN ACTIVITY - First Wi-Fi found......binding");
                        connMgr.BindProcessToNetwork(network);
                    }
                }
            }
        }
    }
}
