using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;
using Acr.UserDialogs;

using qController.ViewModels;
using qController.UI;

using QControlKit;
using QControlKit.Events;
using QControlKit.Constants;
using System;

namespace qController
{
    public partial class WorkspacePage : ContentPage
    {
        private QWorkspace connectedWorkspace;
        private string passcode;
        private Dictionary<string, Grid> cueGridDict = new Dictionary<string, Grid>();
        public WorkspacePage(QWorkspace workspace, string passcode = null)
        {
            InitializeComponent();


            connectedWorkspace = workspace;
            this.passcode = passcode;
            SetupTopBar();

            connectedWorkspace.WorkspaceUpdated += Workspace_WorkspaceUpdated;
            connectedWorkspace.WorkspaceConnectionError += ConnectedWorkspace_WorkspaceConnectionError;
            connectedWorkspace.defaultSendUpdatesOSC = true;
            connectedWorkspace.CueListChangedPlaybackPosition += ConnectedWorkspace_CueListChangedPlaybackPosition;


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            connectedWorkspace.connect(passcode);
        }



        private void SetupTopBar()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            App.rootPage.MenuPage.ChangeToControl();
            App.rootPage.MenuItemSelected += OnMenuItemSelected;

            qNavBar.instanceName.Text = connectedWorkspace.serverName;

        }

        private void ConnectedWorkspace_WorkspaceConnectionError(object source, QWorkspaceConnectionErrorArgs connectionErrorArgs)
        {
            System.Console.WriteLine("WorkspaceConnectionError Called");
            if (connectionErrorArgs.status.Equals("badpass"))
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    InputType = InputType.Number,
                    MaxLength = 4,
                    Title = "Incorrect Passcode!",
                    OkText = "Connect",
                    IsCancellable = true,
                    OnTextChanged = args =>
                    {
                        args.IsValid = args.Value != null && !args.Value.Equals("") && args.Value.Length == 4;
                    },
                    OnAction = (resp) =>
                    {
                        if (resp.Ok)
                        {
                            connectedWorkspace.connect(resp.Value);
                        }   
                    }
                });
            }
        }

        void ConnectedWorkspace_CueListChangedPlaybackPosition(object source, QCueListChangedPlaybackPositionArgs args)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                QCue selectedCue = connectedWorkspace.cueWithID(args.cueID);
                if(selectedCue != null)
                {
                    connectedWorkspace.fetchDefaultPropertiesForCue(selectedCue);
                    selectedCueFrame.BindingContext = new QCueViewModel(selectedCue, false);
                    if (cueGridDict.ContainsKey(args.cueID))
                    {
                        var cueGrid = cueGridDict[args.cueID]; //element to scroll to
                        cueListScrollView.ScrollToAsync(cueGrid, ScrollToPosition.Center, true);
                    }
                }
            });
        }

        void Workspace_WorkspaceUpdated(object source, QWorkspaceUpdatedArgs args)
        {
            Log.Debug("[workspacepage] Workspace Updated");
            App.SetRecentWorkspace(connectedWorkspace);
            if (connectedWorkspace.cueLists.Count > 0)
            {
                List<Task> cueAddTasks = new List<Task>();
                foreach (var aCue in connectedWorkspace.cueLists)
                {
                    if(aCue.cues.Count > 0)
                    {
                        QCueGrid cueGrid = cueToGrid(aCue);

                        cueGridDict.Add(aCue.uid, cueGrid);

                        MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            cueListsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            cueListsGrid.Children.Add(cueGrid, 0, aCue.sortIndex);
                        }).Wait();
                    }
                }

                connectedWorkspace.valueForKey(connectedWorkspace.firstCueList, QOSCKey.PlaybackPositionId); //fetch playback position for cueList once all cue loading is done.

                Device.BeginInvokeOnMainThread(() =>
                {
                    selectedCueFrame.IsVisible = true;
                    cueListScrollView.IsVisible = true;
                });
                //break; //only load first cue list with cues.
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

        QCueGrid cueToGrid(QCue cue)
        {
            QCueGrid cueGrid = new QCueGrid(cue);

            if (cue.cues.Count > 0)
            {
                foreach (var aCue in cue.cues)
                {
                    cueGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var aCueGrid = cueToGrid(aCue);
                    cueGridDict.Add(aCue.uid, aCueGrid);
                    aCueGrid.Margin = new Thickness(0, 0, 0, 0);
                    cueGrid.Children.Add(aCueGrid, 0, aCue.sortIndex + 1);
                    Grid.SetColumnSpan(aCueGrid,cueGrid.ColumnDefinitions.Count);
                }
            }
            return cueGrid;
        }

        void Back()
        {

            if (connectedWorkspace.connected)
                connectedWorkspace.disconnect(); //TODO: This might not be implemented?

            //unsubscribe from events
            App.rootPage.MenuItemSelected -= OnMenuItemSelected;
            connectedWorkspace.WorkspaceUpdated -= Workspace_WorkspaceUpdated;
            connectedWorkspace.CueListChangedPlaybackPosition -= ConnectedWorkspace_CueListChangedPlaybackPosition;
            connectedWorkspace.WorkspaceConnectionError -= ConnectedWorkspace_WorkspaceConnectionError;

            Device.BeginInvokeOnMainThread(() =>
            {
                App.rootPage.MenuPage.ChangeToHome();
                App.NavigationPage.Navigation.PopAsync();
            });
        }

        private void OnMenuItemSelected(object source, MenuEventArgs args)
        {
            
            if (args.Command == "disconnect")
            {
                Back();
            }
        }


        protected async Task WaitAndExecute(int milisec, Action actionToExecute)
        {
            await Task.Delay(milisec);
            actionToExecute();
        }
    }
}