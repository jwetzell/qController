using Xamarin.Forms;
using System;
using Acr.UserDialogs;
using Zeroconf;
using System.Collections.Generic;
using Serilog;
using qController.QItems;
using qController.Communication;
using qController.Cell;
using Xamarin.Essentials;

namespace qController
{
    
    public partial class QConnectionPage : ContentPage
    {
        public QConnectionPage()
        {
            InitializeComponent();
            InitGUI();
            App.rootPage.MenuItemSelected += OnMenuItemSelected;
        }

        private void OnMenuItemSelected(object source, MenuEventArgs args)
        {
            if(args.Command == "scan")
            {
                Scan();
            }
            else if(args.Command == "add")
            {
                AddWorkspace();
            }
            else if (args.Command == "feedback")
            {
                Launcher.OpenAsync(new Uri("mailto:feedback@jwetzell.com?subject=qController%20feedback"));
            }
            else if(args.Command == "support")
            {
                UserDialogs.Instance.Confirm(new ConfirmConfig
                {
                    Title = "Support the Project",
                    Message = "This application will never be a paid app or contain any sort of ads, but if you choose to show your support you may do so by clicking the Donate button below. Thank you!",
                    OkText = "Donate",
                    OnAction = (response) =>
                    {
                        if (response)
                            Launcher.OpenAsync(new Uri("https://linktr.ee/JoelWetzell"));
                    }
                });
            }
        }

        private void InitGUI()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            //ListView Setup
            listView.ItemsSource = QStorage.qInstances;
            listView.ItemTemplate = new DataTemplate(typeof(QInstanceCell));
            listView.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                // don't do anything if we just de-selected the row
                if (e.Item == null) return;
                // do something with e.SelectedItem
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

            //Platform Specific Setup
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    topBar.HeightRequest = App.Height * .09;
                    menuButton.FontSize = App.Height * .04;
                    menuButton.Margin = new Thickness(App.WidthUnit * 2, 0, 0, App.WidthUnit * 2);       
                    break;
                case Device.Android:
                    topBar.HeightRequest = App.Height * .09;
                    menuButton.FontSize = App.Height * .05;
                    menuButton.Margin = new Thickness(App.WidthUnit * 2, 0, 0, App.WidthUnit * 2);          
                    break;
            }

            BackgroundColor = Color.FromHex("4A4A4A");

            //MenuButton Setup
            var menuButtonGesture = new TapGestureRecognizer();
            menuButtonGesture.Tapped += showMenu;
            menuButton.GestureRecognizers.Add(menuButtonGesture);

        }

        void AddWorkspace(){
            
            UserDialogs.Instance.Prompt(new PromptConfig
			{
				Title = "Enter IP of QLab Computer",
				Message = "Enter an IP Address to save",
                OkText = "Next",
				OnTextChanged = args =>
				{
                    //IPAddress ugh
                    args.IsValid = IPHelper.IsValidAddress(args.Value);
				},
				OnAction = (qAddress) =>
				{
                    if (!qAddress.Ok)
                        return;

                    UserDialogs.Instance.Prompt(new PromptConfig
                    {
                        Title="Enter Name",
                        Message = "Enter a Name for " + qAddress.Text,
                        OkText="Save",
                        OnAction = (qName) => {
                            if (!qName.Ok)
                                return;
                            QStorage.AddInstance(qName.Text,qAddress.Text);
                            App.showToast("Manual Workspace Added!");
                        }

                    });
				}
			});
        }

        async void Scan(){
            bool workspacesFound = false;
            App.showToast("Scanning for Instances...");
            Log.Debug("QCONNECTIONPAGE - Begin Scanning");

            IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("_qlab._udp.local.",TimeSpan.FromSeconds(3));
            if(results != null){
                foreach (var result in results)
                {
                    if (result != null)
                    {
                        QInstance instance = new QInstance(result.DisplayName, result.IPAddress);
                        if(QStorage.AddInstance(instance)){
                            Log.Debug($"QCONNECTIONPAGE - {result.DisplayName} @ {result.IPAddress} added");
                            workspacesFound = true;
                        }
                    }
                }
            }

            if(workspacesFound){
                App.showToast("Instance Found and Added!");
            }else{
                App.showToast("No New Instances Found!");
            }
        }

        void showMenu(object sender, EventArgs e)
        {
            App.MenuIsPresented = true;
        }
    }
}
