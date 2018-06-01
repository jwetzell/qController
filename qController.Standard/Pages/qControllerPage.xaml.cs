using Xamarin.Forms;
using System.Net;
using System;
using Rug.Osc;
using Acr.UserDialogs;
using Acr.Settings;
using Zeroconf;
using System.Collections.Generic;

namespace qController
{
    
    public partial class qControllerPage : ContentPage
    {
        public qControllerPage()
        {
            InitializeComponent();
            InitGUI();

        }

        private void InitGUI()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            lstView.ItemsSource = QStorage.qInstances;


            lstView.ItemTemplate = new DataTemplate(typeof(QInstanceCell));
            lstView.SeparatorVisibility = SeparatorVisibility.None;
            lstView.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                // don't do anything if we just de-selected the row
                if (e.Item == null) return;
                // do something with e.SelectedItem
                ((ListView)sender).SelectedItem = null; // de-select the row
            };
            topBar.BackgroundColor = Color.FromHex("71AEFF");

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    scanButton.Margin = new Thickness(20, 45, 20, 10);
                    addButton.Margin = new Thickness(20, 45, 20, 10);
                    break;

            }

            addButton.Source = ImageSource.FromFile("add");
            scanButton.Source = ImageSource.FromFile("scan");

            lstView.BackgroundColor = Color.FromHex("4A4A4A");
            BackgroundColor = Color.FromHex("4A4A4A"); 
            var scanButtonGesture = new TapGestureRecognizer();

            scanButtonGesture.Tapped += Scan;
            scanButton.GestureRecognizers.Add(scanButtonGesture);



            var addButtonGesture = new TapGestureRecognizer();

            addButtonGesture.Tapped += AddWorkspace;
            addButton.GestureRecognizers.Add(addButtonGesture);
        }

        void AddWorkspace(object sender, EventArgs e){
            
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
                            showToast("Manual Workspace Added!");
                        }

                    });
				}
			});
        }

        void showToast(string message){
            var toastConfig = new ToastConfig(message);
            toastConfig.SetDuration(3000);
            toastConfig.SetPosition(ToastPosition.Bottom);
            UserDialogs.Instance.Toast(toastConfig);
        }

        async void Scan(object sender, EventArgs e){
            bool workspacesFound = false;
            showToast("Scanning for Workspaces...");


            Console.WriteLine("Begin Scanning");
            IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("_qlab._udp.local.",TimeSpan.FromSeconds(3));
            if(results != null){
                foreach (var result in results)
                {
                    if (result != null)
                    {
                        QStorage.AddInstance(result.DisplayName, result.IPAddress);
                        Console.WriteLine(result.DisplayName + " @ " + result.IPAddress + " added");
                        workspacesFound = true;
                    }
                }
            }

            if(workspacesFound){
                showToast("Workspace Found and Added!");
            }else{
                showToast("No Workspaces Found!");
            }
        }

    }
}
