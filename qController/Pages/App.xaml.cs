using Acr.UserDialogs;
using qController.Communication;

namespace qController
{
    public partial class App : Application
    {
        public static INavigation iNav;
        public static NavigationPage NavigationPage { get; private set; }
        public static RootPage rootPage;
        public static string QFont;
        public static DisplayInfo mainDisplayInfo;
        public static double Height;
        public static double Width;
        public static double HeightUnit;
        public static double WidthUnit;
        public static QController qControllerToResume;
        public static bool MenuIsPresented
        {
            get
            {
                return rootPage.IsPresented;
            }
            set
            {
                rootPage.IsPresented = value;
            }
        }

        public App()
        {
            InitializeComponent();
            /*MainPage = new NavigationPage(new QConnectionPage());
            iNav = MainPage.Navigation;*/

            mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            Height = mainDisplayInfo.Height / mainDisplayInfo.Density;
            Width = mainDisplayInfo.Width / mainDisplayInfo.Density;
            HeightUnit = Height / 100.0;
            WidthUnit = Width / 100.0;
            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    QFont = "qfont";
                    break;
                case Device.Android:
                    QFont = "qfont.ttf#qfont";
                    break;

            }

            rootPage = new RootPage();
            rootPage.Detail = new NavigationPage(new QConnectionPage());
            MainPage = rootPage;
        }

        public static void showToast(string message)
        {
            var toastConfig = new ToastConfig(message);
            toastConfig.SetDuration(3000);
            toastConfig.SetPosition(ToastPosition.Bottom);
            UserDialogs.Instance.Toast(toastConfig);
        }
    }
}
