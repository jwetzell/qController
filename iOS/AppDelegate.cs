using Foundation;
using UIKit;
using Serilog;

namespace qController.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Log.Logger = new LoggerConfiguration().WriteTo.NSLog().MinimumLevel.Debug().CreateLogger();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
