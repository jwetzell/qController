using Xamarin.Forms;

namespace qController
{
    public partial class App : Application
    {
        public static INavigation iNav;
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new qControllerPage());
            iNav = MainPage.Navigation;
        }

        protected override void OnStart()
        {
            // Handle when your app starts

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
