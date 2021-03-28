﻿using QControlKit;
using Xamarin.Forms;
using Serilog;
using qController.UI.Cells;
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
            qBrowserViewModel.autoUpdate = true;
            serverListView.BindingContext = qBrowserViewModel;
            serverListView.ItemSelected += QWorkspaceSelected;


            //Old way of displaying "QInstances"
            //storageListView.ItemsSource = QStorage.qInstances;
            //storageListView.ItemTemplate = new DataTemplate(typeof(QInstanceCell));
            //storageListView.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            //{
            //    // don't do anything if we just de-selected the row
            //    if (e.Item == null) return;
            //    // do something with e.SelectedItem
            //    ((ListView)sender).SelectedItem = null; // de-select the row
            //};

        }

        private void SetupTopBar()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            App.rootPage.MenuItemSelected += OnMenuItemSelected;
        }

        async void QWorkspaceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            this.qBrowserViewModel.autoUpdate = false;
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
            }
        }

        private void AddInstance(object sender, EventArgs args)
        {
            AddWorkspace();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.qBrowserViewModel.autoUpdate = true;
        }
    }


}