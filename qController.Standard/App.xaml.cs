﻿using Xamarin.Forms;

namespace qController
{
    public partial class App : Application
    {
        public static INavigation iNav;
        public static NavigationPage NavigationPage { get; private set; }
        public static RootPage rootPage;
        public static string QFont;

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
            NavigationPage = new NavigationPage(new QConnectionPage());
            rootPage.Master = menuPage;
            rootPage.Detail = NavigationPage;
            MainPage = rootPage;
            rootPage.Init();
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
