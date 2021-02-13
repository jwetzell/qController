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
    public partial class QWorkspacePage : ContentPage
    {
        private QWorkspace connectedWorkspace;
        private string passcode;
        
        public QWorkspacePage(QWorkspace workspace, string passcode = null)
        {
            InitializeComponent();

            connectedWorkspace = workspace;
            this.passcode = passcode;

            QCueGridListHelper.reset();

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
            if (connectionErrorArgs.status.Equals(QConnectionStatus.BadPass))
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
                    //update properties for selected cue to ensure latest data for cue preview
                    connectedWorkspace.fetchDefaultPropertiesForCue(selectedCue);
                    
                    selectedCueFrame.BindingContext = new QCueViewModel(selectedCue, false);

                    //Find cue grid for selected cue and scroll to it if it exists
                    QCueGrid gridToScrollTo = QCueGridListHelper.get(args.cueID);
                    if(gridToScrollTo != null)
                    {
                        cueListScrollView.ScrollToAsync(gridToScrollTo, ScrollToPosition.Center,true);
                    }
                }
                else
                {
                    //Setup an "empty" cue for when no cue is selected
                    QCue emptyCue = new QCue();
                    emptyCue.workspace = connectedWorkspace;
                    emptyCue.type = QCueType.Memo;
                    emptyCue.listName = "No Cue Selected";
                    
                    selectedCueFrame.BindingContext = new QCueViewModel(emptyCue, false);
                }
            });
        }

        void Workspace_WorkspaceUpdated(object source, QWorkspaceUpdatedArgs args)
        {
            Log.Debug("[workspacepage] Workspace Updated");

            //Workspace updated means we connected so update the recent workspace
            App.SetRecentWorkspace(connectedWorkspace);

            if (connectedWorkspace.cueLists.Count > 0)
            {
                List<Task> cueAddTasks = new List<Task>();

                
                foreach (var aCue in connectedWorkspace.cueLists)
                {
                    
                    if(aCue.cues.Count > 0)
                    {
                        QCueGrid mainCueGrid = new QCueGrid(aCue);
                        MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            QCueGridListHelper.insert(aCue.uid, mainCueGrid);
                            cueListsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            cueListsGrid.Children.Add(mainCueGrid, 0, aCue.sortIndex);
                        }).Wait();
                    }
                    break; //TODO: Make the displayed cue list selectable
                }

                //TODO: fetch playback position for first cue list once all cue loading is done.
                connectedWorkspace.valueForKey(connectedWorkspace.firstCueList, QOSCKey.PlaybackPositionId);


                //Display cuelist and selected cue info
                Device.BeginInvokeOnMainThread(() =>
                {
                    selectedCueFrame.IsVisible = true;
                    cueListScrollView.IsVisible = true;
                });
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }

        void Back()
        {

            if (connectedWorkspace.connected)
                connectedWorkspace.disconnect(); //TODO: This might not be implemented?

            //purge QCueGridListHelper
            QCueGridListHelper.reset();

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