using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Essentials;
using qController.Communication;
using qController.Pages;
using System;
using QControlKit;
using Serilog;

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
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
            SetAppResources();

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

            rootPage = new RootPage();
            NavigationPage = new NavigationPage(new QBrowserPage());
            rootPage.Detail = NavigationPage;
            MainPage = rootPage;
            rootPage.Init();

            AppActions.OnAppAction += AppActions_OnAppAction;

        }

        private void AppActions_OnAppAction(object sender, AppActionEventArgs e)
        {
            // Don't handle events fired for old application instances
            // and cleanup the old instance's event handler
            if (Current != this && Current is App app)
            {
                AppActions.OnAppAction -= app.AppActions_OnAppAction;
                return;
            }
            Device.BeginInvokeOnMainThread(async () =>
            {
                QRecentWorkspaceInfo packagedInfo = QStorage.recentWorkspaceInfo;
                if (packagedInfo != null && e.AppAction.Id.Equals("recent_workspace"))
                {
                    await NavigationPage.PopToRootAsync();
                    QWorkspace recentWorkspace = new QWorkspace(packagedInfo.workspaceInfo, new QServer(packagedInfo.serverInfo.host, packagedInfo.serverInfo.port));
                    await NavigationPage.PushAsync(new QWorkspacePage(recentWorkspace));
                    Log.Debug("Pushed WorkspacePage from App Action");
                }
            });
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            SetAppResources();
        }

        private void SetAppResources()
        {

            switch (App.Current.RequestedTheme)
            {
                case OSAppTheme.Light:
                    SetLightResources();
                    break;
                case OSAppTheme.Dark:
                    SetDarkResources();
                    break;
                case OSAppTheme.Unspecified:
                    //Default to Light
                    SetLightResources();
                    break;
            }
        }

        private void SetDarkResources()
        {

            Resources["NavigationBarColor"] = Color.FromHex("#3f6dab");
            Resources["PageBackgroundColor"] = Color.FromHex("#525252");


            Resources["PrimaryColor"] = Color.WhiteSmoke;
            Resources["SecondaryColor"] = Color.White;
            Resources["PrimaryTextColor"] = Color.LightGray;
            Resources["SecondaryTextColor"] = Color.White;
            Resources["TertiaryTextColor"] = Color.Gray;
            Resources["TransparentColor"] = Color.Transparent;

            Resources["IconTextColor"] = Color.WhiteSmoke;

            Resources["ListViewBackgroundColor"] = Color.FromHex("#363636");

            Resources["ServerCellBackgroundColor"] = Color.FromHex("#363636");
            Resources["WorkspaceCellBackgroundColor"] = Color.FromHex("#737373");

            Resources["SelectedCueCellBackgroundColor"] = Color.FromHex("#737373");

        }

        private void SetLightResources()
        {
            Resources["NavigationBarColor"] = Color.FromHex("#3089ff");
            Resources["PageBackgroundColor"] = Color.LightGray;


            Resources["PrimaryColor"] = Color.WhiteSmoke;
            Resources["SecondaryColor"] = Color.White;
            Resources["PrimaryTextColor"] = Color.Black;
            Resources["SecondaryTextColor"] = Color.White;
            Resources["TertiaryTextColor"] = Color.Gray;
            Resources["TransparentColor"] = Color.Transparent;

            Resources["IconTextColor"] = Color.Black;

            Resources["ListViewBackgroundColor"] = Color.White;

            Resources["ServerCellBackgroundColor"] = Color.White;
            Resources["WorkspaceCellBackgroundColor"] = Color.FromHex("#D8D8D8");

            Resources["SelectedCueCellBackgroundColor"] = Color.FromHex("#D8D8D8");

        }

        public static void SetRecentWorkspace(QWorkspace workspace)
        {
            try
            {
                QRecentWorkspaceInfo recentWorkspaceInfo = new QRecentWorkspaceInfo {
                    workspaceInfo = workspace.getWorkspaceInfo(),
                    serverInfo = workspace.GetServerInfo()
                };

                QStorage.UpdateRecentWorkspace(recentWorkspaceInfo);

                Device.BeginInvokeOnMainThread(async () =>
                {
                    await AppActions.SetAsync(new AppAction("recent_workspace", workspace.name, workspace.version));
                });
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        public static void showToast(string message)
        {
            var toastConfig = new ToastConfig(message);
            toastConfig.SetDuration(3000);
            toastConfig.SetPosition(ToastPosition.Bottom);
            UserDialogs.Instance.Toast(toastConfig);
        }

        public static void ShowMenu(object sender, EventArgs e)
        {
            App.MenuIsPresented = true;
        }
    }
}
