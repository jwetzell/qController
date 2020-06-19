using QSharp;
using System.Collections.Generic;
using Xamarin.Forms;
using Serilog;
using System.Collections.ObjectModel;
using qController.Cell;
using Acr.UserDialogs;
using qController.Dialogs;
using Xamarin.Essentials;
using System;

namespace qController.Pages
{
    public partial class QBrowserPage : ContentPage
    {
        public ObservableCollection<ServerGroup> servers = new ObservableCollection<ServerGroup>();
        public QBrowserPage()
        {
            InitializeComponent();
            App.rootPage.MenuItemSelected += OnMenuItemSelected;

            serverListView.ItemsSource = servers;
            QBrowser browser = new QBrowser();
            browser.ServerUpdatedWorkspaces += Browser_ServerUpdatedWorkspaces;
            serverListView.ItemSelected += ServerListView_ItemSelected;

            storageListView.ItemsSource = QStorage.qInstances;
            storageListView.ItemTemplate = new DataTemplate(typeof(QInstanceCell));
            storageListView.ItemTapped += (object sender, ItemTappedEventArgs e) =>
            {
                // don't do anything if we just de-selected the row
                if (e.Item == null) return;
                // do something with e.SelectedItem
                ((ListView)sender).SelectedItem = null; // de-select the row
            };

        }

        async void ServerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            QWorkspace selectedWorkspace = e.SelectedItem as QWorkspace;
            Log.Debug($"[demo] workspace: {selectedWorkspace.nameWithoutPathExtension} has been selected");
            ((ListView)sender).SelectedItem = null;
            await Navigation.PushAsync(new WorkspacePage(selectedWorkspace));
        }

        private void Browser_ServerUpdatedWorkspaces(object source, QServerUpdatedArgs args)
        {
            ServerGroup serverGroup = new ServerGroup(args.server.name);
            serverGroup.AddRange(args.server.workspaces);
            Device.BeginInvokeOnMainThread(() =>
            {
                servers.Add(serverGroup);
            });
        }


        void AddWorkspace()
        {
            UserDialogs.Instance.Prompt(new AddInstancePrompt());
        }

        private void OnMenuItemSelected(object source, MenuEventArgs args)
        {
            if (args.Command == "add")
            {
                AddWorkspace();
            }
            else if (args.Command == "feedback")
            {
                Launcher.OpenAsync(new Uri("mailto:feedback@jwetzell.com?subject=qController%20feedback"));
            }
            else if (args.Command == "support")
            {
                UserDialogs.Instance.Confirm(new DonatePrompt());
            }
        }
    }

    public class ServerGroup : List<QWorkspace>
    {
        public string name { get; set; }
        public ServerGroup(string name)
        {
            this.name = name;
        }
    }
}