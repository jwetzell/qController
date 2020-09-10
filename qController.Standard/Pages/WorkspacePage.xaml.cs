using System.Collections.Generic;
using System.Threading.Tasks;
using QControlKit;
using qController.ViewModels;
using Serilog;
using Xamarin.Essentials;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Windows.Input;

namespace qController
{
    public partial class WorkspacePage : ContentPage
    {
        private QWorkspace connectedWorkspace;
        private Dictionary<string, Grid> cueGridDict = new Dictionary<string, Grid>();
        public WorkspacePage(QWorkspace workspace, string passcode = null)
        {
            InitializeComponent();

            connectedWorkspace = workspace;
            connectedWorkspace.WorkspaceUpdated += Workspace_WorkspaceUpdated;
            connectedWorkspace.WorkspaceConnectionError += ConnectedWorkspace_WorkspaceConnectionError;
            connectedWorkspace.defaultSendUpdatesOSC = true;
            connectedWorkspace.CueListChangedPlaybackPosition += ConnectedWorkspace_CueListChangedPlaybackPosition;

            connectedWorkspace.connect(passcode);
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
                selectedCueGrid.BindingContext = new QCueViewModel(connectedWorkspace.cueWithID(args.cueID), false);

                if (cueGridDict.ContainsKey(args.cueID))
                {
                    var cueGrid = cueGridDict[args.cueID]; //element to scroll to
                    cueListScrollView.ScrollToAsync(cueGrid, ScrollToPosition.Center, true);
                }
            });
        }

        void Workspace_WorkspaceUpdated(object source, QWorkspaceUpdatedArgs args)
        {
            Log.Debug("[workspacepage] Workspace Updated");

            foreach (var cue in connectedWorkspace.cueLists)
            {
                if (cue.cues.Count > 0)
                {
                    List<Task> cueAddTasks = new List<Task>();
                    foreach (var aCue in cue.cues)
                    {
                        Grid cueGrid = cueToGrid(aCue);
                        cueGridDict.Add(aCue.uid, cueGrid);
                        MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            cueListsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                            cueListsGrid.Children.Add(cueGrid, 0, aCue.sortIndex);
                        }).Wait();
                    }
                    connectedWorkspace.valueForKey(cue, QOSCKey.PlaybackPositionId); //fetch playback position for cueList once all cue loading is done.
                    break; //only load first cue list with cues.
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //unsubscribe from events
            connectedWorkspace.WorkspaceUpdated -= Workspace_WorkspaceUpdated;
            connectedWorkspace.CueListChangedPlaybackPosition -= ConnectedWorkspace_CueListChangedPlaybackPosition;
            connectedWorkspace.WorkspaceConnectionError -= ConnectedWorkspace_WorkspaceConnectionError;
            //disconnect
            connectedWorkspace.disconnect();

        }

        Grid cueToGrid(QCue cue)
        {
            Grid cueGrid = new Grid { RowSpacing = 0 };
            QCueViewModel qCueViewModel = new QCueViewModel(cue, true);
            cueGrid.RowDefinitions = new RowDefinitionCollection();
            cueGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });


            //Group List "Frame" added early so other children or "on top"
            if (cue.cues.Count > 0)
            {
                var cueFrame = new Frame
                {
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.Black
                };
                cueGrid.Children.Add(cueFrame);
                Grid.SetRowSpan(cueFrame, cue.cues.Count + 1);
            }

            var cueLabel = new Label
            {
                BindingContext = qCueViewModel,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
            };
            cueLabel.SetBinding(Label.TextProperty, "name", BindingMode.OneWay);

            //System.Console.WriteLine($"Cue has continue mode: {cue.}" );
            //Section for selecting a cue by tapping the name Label
            var selectCueGesture = new TapGestureRecognizer();
            selectCueGesture.Tapped += (sender, e) =>
            {
                connectedWorkspace.firstCueList.playbackPositionID = cue.uid;
            };
            cueLabel.GestureRecognizers.Add(selectCueGesture);


            var cueBackground = new Frame
            {
                BindingContext = qCueViewModel,
                Opacity = 0.50,
                HasShadow = false,
                CornerRadius = 0,
                BorderColor = Color.Black
            };

            cueBackground.SetBinding(BackgroundColorProperty, "color", BindingMode.OneWay);

            var cueSelectedIndicator = new Frame
            {
                BindingContext = qCueViewModel,
                BackgroundColor = Color.Blue,
                HasShadow = false
            };

            cueSelectedIndicator.SetBinding(IsVisibleProperty, "IsSelected");

            cueGrid.Children.Add(cueSelectedIndicator, 0, 0);
            cueGrid.Children.Add(cueBackground, 0, 0);
            cueGrid.Children.Add(cueLabel, 0, 0);

            if (cue.cues.Count > 0)
            {
                foreach (var aCue in cue.cues)
                {
                    cueGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    var aCueGrid = cueToGrid(aCue);
                    cueGridDict.Add(aCue.uid, aCueGrid);
                    aCueGrid.Margin = new Thickness(10, 0, 0, 0);
                    cueGrid.Children.Add(aCueGrid, 0, aCue.sortIndex + 1);
                }

                

            }
            return cueGrid;
        }
    }
}