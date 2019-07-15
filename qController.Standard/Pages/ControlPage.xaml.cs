using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections;
using Acr.UserDialogs;
namespace qController
{
    public partial class ControlPage : ContentPage
    {
        QController qController;
        QSelectedCueCell qCell;
        QSelectedCueOptionsCell qSelectedCueOptions;
        Grid mainG;
        QControlsBlock qControlsBlock;
        public ControlPage(string name, string address)
        {
            InitializeComponent();

            qController = new QController(address, 53000);
            qController.qClient.qParser.WorkspaceInfoReceived += WorkspaceInfoReceived;
            qController.qClient.qParser.WorkspaceUpdated += WorkspaceUpdated;
            qController.qClient.qParser.PlaybackPositionUpdated += PlaybackPositionUpdated;
            qController.qClient.qParser.CueInfoUpdated += OnCueUpdateReceived;
            qController.qClient.qParser.ChildrenUpdated += OnChildrenUpdated;
            App.rootPage.MenuItemSelected += OnMenuItemSelected;

            instanceName.Text = name;

            InitGUI();

            qController.KickOff();
        }

        private void WorkspaceInfoReceived(object source, WorkspaceInfoArgs args)
        {
            if(args.WorkspaceInfo.Count > 1)
            {
                Console.WriteLine("MULTIPLE WORKSPACES ON SELECTED COMPUTER");
                PromptForWorkspace(args.WorkspaceInfo);
            }
            else
            {
                Console.WriteLine("ONLY ONE WORKSPACE ON SELECTED COMPUTER");
                qController.Connect(args.WorkspaceInfo[0].uniqueID);

                Device.BeginInvokeOnMainThread(() => {
                    FinishUI();
                });
            }
        }

        private void PromptForWorkspace(List<QInfo> workspaces)
        {
            ActionSheetConfig config = new ActionSheetConfig();
            config.SetTitle("Select Workspace");
            for (int i = 0; i < workspaces.Count; i++)
            {
                QInfo workspace = workspaces[i];
                config.Add(workspace.displayName, new Action(() => { 
                    Console.WriteLine("Workspace Selected: " + workspace.displayName);
                    qController.Connect(workspace.uniqueID);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        FinishUI();
                    });
                }));
            }
            UserDialogs.Instance.ActionSheet(config);
        }

        private void InitGUI()
        {
            App.rootPage.MenuPage.ChangeToControl();
            NavigationPage.SetHasNavigationBar(this, false);

            topBar.BackgroundColor = Color.FromHex("71AEFF");


            BackgroundColor = Color.FromHex("4A4A4A");
            menuButton.Text = QIcon.MENU;
 
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    topBar.HeightRequest = Math.Max(App.Height * .09, 65);
                    menuButton.Margin = new Thickness(App.WidthUnit * 2, 0, 0, App.WidthUnit * 2);
                    menuButton.FontSize = App.Height * .04;
                    break;
                case Device.Android:
                    topBar.HeightRequest = App.Height * .08;
                    menuButton.Margin = new Thickness(App.WidthUnit * 2, 0, 0, App.WidthUnit * 2);
                    menuButton.FontSize = App.Height * .05;
                    break;
            }
            var menuButtonGesture = new TapGestureRecognizer();

            menuButtonGesture.Tapped += ShowMenu;
            menuButton.GestureRecognizers.Add(menuButtonGesture);


            qCell = new QSelectedCueCell();
            sLayout.Children.Add(qCell);
        }

        void FinishUI()
        {
            string workspace_prefix = "/workspace/" + qController.qWorkspace.workspace_id;
            
            qSelectedCueOptions = new QSelectedCueOptionsCell();

            qSelectedCueOptions.mainSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs(workspace_prefix + "/cue_id/" + qSelectedCueOptions.activeCue + "/sliderLevel/0", (float)args.NewValue);
            };
            qSelectedCueOptions.leftSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs(workspace_prefix + "/cue_id/" + qSelectedCueOptions.activeCue + "/sliderLevel/1", (float)args.NewValue);
            };
            qSelectedCueOptions.rightSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs(workspace_prefix + "/cue_id/" + qSelectedCueOptions.activeCue + "/sliderLevel/2", (float)args.NewValue);
            };

            qControlsBlock = new QControlsBlock(qController);
            
            sLayout.Children.Add(qControlsBlock);
        }

        void ShowMenu(object sender, EventArgs e)
        {
            //App.NavigationPage.Navigation.PopAsync();
            App.MenuIsPresented = true;
        }

        void Back()
        {
            qController.Disconnect();
            App.rootPage.MenuPage.ChangeToHome();
            App.NavigationPage.Navigation.PopAsync();
        }

        public void WorkspaceUpdated(object sender, WorkspaceEventArgs e)
        {
            if(e.UpdatedWorkspace.data.Count > 0)
            {
                qController.qWorkspace = e.UpdatedWorkspace;
                qController.qWorkspace.CheckPopulated();

                if (qController.qWorkspace.IsPopulated)
                {
                    App.rootPage.MenuPage.ChangeToWorkspace(e.UpdatedWorkspace);
                    if (qCell.activeCue == null)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            QCue noSelect = new QCue();
                            noSelect.listName = "No Cue Selected";
                            noSelect.type = "";
                            noSelect.notes = "Workspace has loaded but no cue is selected";
                            noSelect.number = "!";
                            qCell.UpdateSelectedCue(noSelect);
                        });
                        qController.qClient.UpdateSelectedCue(qController.qWorkspace.workspace_id);
                    }
                    Console.WriteLine("Workspace updated in ControlPage: " + qController.qWorkspace.workspace_id);
                }
            }
        }

        public void PlaybackPositionUpdated(object sender, PlaybackPositionArgs e)
        {
            qController.playbackPosition = e.PlaybackPosition;
            Console.WriteLine("Playback Position updated in ControlPage: " + qController.playbackPosition);
            QCue cue = qController.qWorkspace.GetCue(qController.playbackPosition);
            if(cue != null)
            {
                Device.BeginInvokeOnMainThread(() => {
                    if (cue.type.Equals("Audio"))
                        sLayout.Children.Add(qSelectedCueOptions);
                    else
                        sLayout.Children.Remove(qSelectedCueOptions);
                    qCell.UpdateSelectedCue(cue);
                });
            }
        }

        public void OnCueUpdateReceived(object sender, CueEventArgs args)
        {
            qController.qWorkspace.UpdateCue(args.Cue);
            App.rootPage.MenuPage.ChangeCueName(args.Cue.uniqueID, args.Cue.listName);
            if (qController.playbackPosition == null)
            {
                qController.playbackPosition = args.Cue.uniqueID;
            }

            if (args.Cue.uniqueID == qController.playbackPosition)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Console.WriteLine("Refreshing Currently Displayed Cue");
                    if (args.Cue.levels != null)
                        sLayout.Children.Add(qSelectedCueOptions);
                    else
                        sLayout.Children.Remove(qSelectedCueOptions);
                    qCell.UpdateSelectedCue(args.Cue);
                    //if (!mainG.IsVisible)
                      //  mainG.IsVisible = true;
                    if(qSelectedCueOptions != null)
                    {
                        qSelectedCueOptions.activeCue = args.Cue.uniqueID;
                        if (args.Cue.levels != null)
                        {
                            qSelectedCueOptions.UpdateLevels(args.Cue.levels[0]);
                        }
                    }
                });
            }
        }

        private void OnChildrenUpdated(object source, ChildrenEventArgs args)
        {
            Console.WriteLine("Children Updated in ControlPage: " + args.cue_id);
            qController.qWorkspace.UpdateChildren(args.cue_id, args.children);
            Device.BeginInvokeOnMainThread(() =>
            {
                App.rootPage.MenuPage.ChangeToControl();
                App.rootPage.MenuPage.ChangeToWorkspace(qController.qWorkspace);
            });
        }

        private void OnMenuItemSelected(object source, MenuEventArgs args)
        {
            if (args.Command.Contains("/"))
            {
                qController.qClient.sendStringUDP(args.Command);
            }
            else if (args.Command == "disconnect")
            {
                Back();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Console.WriteLine("Disappeared");
            qController.Kill();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("Appeared");
        }
    }
}
