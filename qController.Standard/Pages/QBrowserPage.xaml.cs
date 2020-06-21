using QSharp;
using Xamarin.Forms;
using Serilog;
using System.Collections.ObjectModel;
using qController.Cell;
using Acr.UserDialogs;
using qController.Dialogs;
using Xamarin.Essentials;
using System;
using qController.ViewModels;

namespace qController.Pages
{
    public partial class QBrowserPage : ContentPage
    {
        public QBrowserPage()
        {
            InitializeComponent();
            App.rootPage.MenuItemSelected += OnMenuItemSelected;

            serverListView.BindingContext = new QBrowserViewModel(new QBrowser());

            
            serverListView.ItemSelected += QWorkspaceSelected;



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

        async void QWorkspaceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
            QWorkspace selectedWorkspace = (e.SelectedItem as QWorkspaceViewModel).workspace;
            Log.Debug($"[demo] workspace: {selectedWorkspace.nameWithoutPathExtension} has been selected");
            ((ListView)sender).SelectedItem = null;
            await Navigation.PushAsync(new WorkspacePage(selectedWorkspace));
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

    
}