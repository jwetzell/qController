using QControlKit;
using Xamarin.Forms;
using Serilog;
using Acr.UserDialogs;
using qController.UI.Dialogs;
using Xamarin.Essentials;
using System;
using qController.ViewModels;

namespace qController.Pages
{
    public partial class QBrowserPage : ContentPage
    {
        private QBrowserViewModel qBrowserViewModel;
        public QBrowserPage()
        {
            InitializeComponent();

            SetupTopBar();
            qBrowserViewModel = new QBrowserViewModel(new QBrowser());
            serverListView.BindingContext = qBrowserViewModel;
            serverListView.ItemSelected += QWorkspaceSelected;

        }

        private void SetupTopBar()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            App.rootPage.MenuItemSelected += OnMenuItemSelected;
        }

        async void QWorkspaceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) {
                return;
            }

            qBrowserViewModel.autoUpdate = false;
            QWorkspace selectedWorkspace = (e.SelectedItem as QWorkspaceViewModel).workspace;
            Log.Debug($"[demo] workspace: {selectedWorkspace.nameWithoutPathExtension} has been selected");
            ((ListView)sender).SelectedItem = null;

            if (selectedWorkspace.hasPasscode)
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    InputType = InputType.Number,
                    MaxLength = 4,
                    Title = "Workspace Requires Passcode",
                    OkText = "Connect",
                    IsCancellable = true,
                    OnTextChanged = args =>
                    {
                        args.IsValid = args.Value != null && !args.Value.Equals("") && args.Value.Length == 4;
                    },
                    OnAction = async (resp) =>
                    {
                        if (resp.Ok)
                        {
                            await Navigation.PushAsync(new QWorkspacePage(selectedWorkspace, resp.Value));
                        }
                    }

                });
            }
            else
            {
                await Navigation.PushAsync(new QWorkspacePage(selectedWorkspace));
            }
        }

        void AddWorkspace()
        {
            UserDialogs.Instance.Prompt(new AddInstancePrompt());
        }

        private void OnMenuItemSelected(object source, MenuEventArgs args)
        {
            if (args.Command == "feedback")
            {
                Launcher.OpenAsync(new Uri("mailto:feedback@jwetzell.com?subject=qController%20feedback"));
            }
            else if (args.Command == "support")
            {
                UserDialogs.Instance.Confirm(new DonatePrompt());
            }else if(args.Command == "scan")
            {
                qBrowserViewModel.InitiateScan();
                App.showToast("Scan Initiated.........");
            }
        }

        private void AddInstance(object sender, EventArgs args)
        {
            AddWorkspace();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            qBrowserViewModel.autoUpdate = true;
        }
    }


}