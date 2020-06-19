using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Essentials;
using qController.Communication;
using qController.Pages;

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
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    QFont = "qfont";
                    break;
                case Device.Android:
                    QFont = "qfont.ttf#qfont";
                    break;

            }

            var menuPage = new MenuPage();
            rootPage = new RootPage();
            NavigationPage = new NavigationPage(new QBrowserPage());
            rootPage.Master = menuPage;
            rootPage.Detail = NavigationPage;
            MainPage = rootPage;
            rootPage.Init();
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
