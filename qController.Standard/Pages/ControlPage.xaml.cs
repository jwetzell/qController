using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Serilog;
using Acr.UserDialogs;
using qController.Dialogs;
using qController.QItems;
using qController.Cell;
using qController.Communication;

namespace qController
{
    public partial class ControlPage : ContentPage
    {
        QController qController;
        QSelectedCueCell qCell;
        QLevelsCell qLevelsCell;
        QCueListCell qCueListCell;
        QLevelsButton showLevelsButton;
        QControlsBlock qControlsBlock;
        WorkspacePrompt workspacePrompt = new WorkspacePrompt();

        public ControlPage(string name, string address)
        {
            InitializeComponent();

            qController = new QController(address, 53000);
            qController.qClient.qParser.WorkspaceInfoReceived += WorkspaceInfoReceived;
            qController.qClient.qParser.WorkspaceUpdated += WorkspaceUpdated;
            qController.qClient.qParser.WorkspaceDisconnect += WorkspaceDisconnected;
            qController.qClient.qParser.PlaybackPositionUpdated += PlaybackPositionUpdated;
            qController.qClient.qParser.ConnectionStatusChanged += OnConnectionStatusChanged;
            qController.qClient.qParser.CueInfoUpdated += OnCueUpdateReceived;
            qController.qClient.qParser.ChildrenUpdated += OnChildrenUpdated;
            workspacePrompt.WorkspaceSelected += OnWorkspaceSelected;

            App.rootPage.MenuItemSelected += OnMenuItemSelected;

            instanceName.Text = name;

            InitGUI();

            if (!qController.qClient.connected)
            {
                App.showToast("Error connecting...make sure QLab is running");
                Back();
            }
            else
            {
                qController.KickOff();
            }
        }

        private void OnWorkspaceSelected(object source, WorkspacePromptArgs args)
        {
            if (args.SelectedWorkspace != null)
            {
                qController.Connect(args.SelectedWorkspace);
            }
            else
            {
                Log.Debug("CONTROLPAGE - No Workspace selected backing out");
                Back();
            }
        }

        private void OnConnectionStatusChanged(object source, ConnectEventArgs args)
        {
            Log.Debug($"CONTROLPAGE - Connection Status Changed: {args.WorkspaceId} : {args.Status}");
            if (args.Status.Equals("ok"))
            {
                Device.BeginInvokeOnMainThread(() => {
                    FinishUI();
                });
            }
            else if (args.Status.Equals("badpass"))
            {
                workspacePrompt.promptWorkspacePasscode(args.WorkspaceId);
            }
        }

        private void WorkspaceInfoReceived(object source, WorkspaceInfoArgs args)
        {
            if (args.WorkspaceInfo.Count > 1)
            {
                Log.Debug("CONTROLPAGE - MULTIPLE WORKSPACES ON SELECTED COMPUTER");
                PromptForWorkspace(args.WorkspaceInfo);
            }
            else if (args.WorkspaceInfo.Count == 1)
            {
                Log.Debug("CONTROLPAGE - ONLY ONE WORKSPACE ON SELECTED COMPUTER");
                if (!args.WorkspaceInfo[0].hasPasscode)
                {
                    qController.Connect(args.WorkspaceInfo[0].uniqueID);

                }
                else
                {
                    workspacePrompt.promptWorkspacePasscode(args.WorkspaceInfo[0].uniqueID);
                }

            }
            else
            {
                NoWorkspacesConfig noWorkspacesConfig = new NoWorkspacesConfig(NoWorkspaceDetected);
                UserDialogs.Instance.Confirm(noWorkspacesConfig);
            }
        }

        private void PromptForWorkspace(List<QWorkspaceInfo> workspaces)
        {

            UserDialogs.Instance.ActionSheet(workspacePrompt.getActionSheetConfigForWorkspaces(workspaces));
        }

        private void InitGUI()
        {
            App.rootPage.MenuPage.ChangeToControl();
            NavigationPage.SetHasNavigationBar(this, false);

            qCell = new QSelectedCueCell();

            qCell.SelectedCueEdited += OnSelectedCueEdited;

            AbsoluteLayout.SetLayoutFlags(qCell, AbsoluteLayoutFlags.All);

            instanceName.FontSize = App.HeightUnit * 3;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    AbsoluteLayout.SetLayoutBounds(qCell, new Rectangle(0, 0.13, 1, 0.30));
                    topBar.HeightRequest = App.Height * .09;
                    menuButton.FontSize = App.Height * .04;
                    break;
                case Device.Android:
                    AbsoluteLayout.SetLayoutBounds(qCell, new Rectangle(0, 0.13, 1, 0.35));
                    topBar.HeightRequest = App.Height * .06;
                    menuButton.FontSize = App.Height * .05;
                    break;
            }

            //Menu Button Setup
            var menuButtonGesture = new TapGestureRecognizer();

            menuButtonGesture.Tapped += ShowMenu;
            menuButton.GestureRecognizers.Add(menuButtonGesture);
            menuButton.Margin = new Thickness(App.WidthUnit * 2, 0, 0, App.WidthUnit * 2);

            sLayout.Children.Add(qCell);
        }

        private void SendOSCFromButton(object sender, EventArgs args)
        {
            if (((QButton)sender).qCommand.type == "WORKSPACE")
            {
                string workspace_prefix = "/workspace/" + qController.qWorkspace.workspace_id;
                string command = workspace_prefix + ((QButton)sender).qCommand.osc;
                qController.qClient.sendTCP(command);
            }
        }

        private void OnSelectedCueEdited(object source, CueEditArgs args)
        {
            string address = "/workspace/" + qController.qWorkspace.workspace_id + "/cue_id/" + args.CueID + "/" + args.Property;
            qController.qClient.sendTCP(address, args.NewValue);
        }

        void FinishUI()
        {
            string workspace_prefix = "/workspace/" + qController.qWorkspace.workspace_id;

            qLevelsCell = new QLevelsCell();

            qLevelsCell.mainSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendTCP(workspace_prefix + "/cue_id/" + qLevelsCell.activeCue + "/sliderLevel/0", (float)args.NewValue);
            };
            qLevelsCell.leftSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendTCP(workspace_prefix + "/cue_id/" + qLevelsCell.activeCue + "/sliderLevel/1", (float)args.NewValue);
            };
            qLevelsCell.rightSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendTCP(workspace_prefix + "/cue_id/" + qLevelsCell.activeCue + "/sliderLevel/2", (float)args.NewValue);
            };


            showLevelsButton = new QLevelsButton();
            showLevelsButton.button.Clicked += ToggeQLevelsCellVisiblity;

            qControlsBlock = new QControlsBlock(SendOSCFromButton);

            AbsoluteLayout.SetLayoutFlags(qControlsBlock, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutFlags(qLevelsCell, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutFlags(showLevelsButton, AbsoluteLayoutFlags.PositionProportional);


            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    AbsoluteLayout.SetLayoutBounds(qControlsBlock, new Rectangle(0, 0.53, 1, 0.25));
                    AbsoluteLayout.SetLayoutBounds(qLevelsCell, new Rectangle(0.9, 0.40, 0.8, 0.25));
                    AbsoluteLayout.SetLayoutBounds(showLevelsButton, new Rectangle(0.02, 0.34, App.HeightUnit * 8, App.HeightUnit * 8));
                    break;
                case Device.Android:
                    AbsoluteLayout.SetLayoutBounds(qControlsBlock, new Rectangle(0, 0.58, 1, 0.25));
                    AbsoluteLayout.SetLayoutBounds(qLevelsCell, new Rectangle(0.9, 0.45, 0.8, 0.25));
                    AbsoluteLayout.SetLayoutBounds(showLevelsButton, new Rectangle(0.02, 0.39, App.HeightUnit * 8, App.HeightUnit * 8));
                    break;
            }

            sLayout.Children.Add(qControlsBlock);
            sLayout.Children.Add(qLevelsCell);
            sLayout.Children.Add(showLevelsButton);
        }

        private void ToggeQLevelsCellVisiblity(object sender, EventArgs e)
        {
            qLevelsCell.IsVisible = !qLevelsCell.IsVisible;
        }

        void ShowMenu(object sender, EventArgs e)
        {
            
            App.MenuIsPresented = true;
        }

        void Back()
        {
            if(qController.qClient.connected)
                qController.Kill();
            App.rootPage.MenuItemSelected -= OnMenuItemSelected;
            Device.BeginInvokeOnMainThread(() =>
            {
                App.rootPage.MenuPage.ChangeToHome();
                App.NavigationPage.Navigation.PopAsync();
            });
        }

        public void NoWorkspaceDetected(bool resp)
        {
            if (resp)
                Back();
        }
        public void WorkspaceDisconnected(object sender, EventArgs e)
        {
            Back();
        }

        public void WorkspaceUpdated(object sender, WorkspaceEventArgs e)
        {
            if(e.UpdatedWorkspace.data.Count > 0)
            {
                qController.qWorkspace = e.UpdatedWorkspace;
                qController.qWorkspace.CheckPopulated();

                if (qController.qWorkspace.IsPopulated)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.rootPage.MenuPage.ChangeToWorkspace(e.UpdatedWorkspace);
                    }); 
                    if (qCell.activeCue == null)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            qCell.UpdateSelectedCue(new NoQCueSelected());
                        });
                        Log.Debug("CONTROLPAGE - Update Selected Cue Called because of Inital Workspace Load");
                        qController.qClient.UpdateSelectedCue(qController.qWorkspace.workspace_id);
                    }
                    Log.Debug("CONTROLPAGE - Workspace Updated " + qController.qWorkspace.workspace_id);
                }
            }
        }

        public void PlaybackPositionUpdated(object sender, PlaybackPositionArgs e)
        {
            qController.playbackPosition = e.PlaybackPosition;
            Log.Debug("CONTROLPAGE - Playback Position Updated " + qController.playbackPosition);
            QCue cue = qController.qWorkspace.GetCue(qController.playbackPosition);
            if(cue != null)
            {
                Device.BeginInvokeOnMainThread(() => {
                    if (cue.levels != null)
                        showLevelsButton.IsVisible = true;
                    else
                    {
                        showLevelsButton.IsVisible = false;
                        qLevelsCell.IsVisible = false;
                    }
                    qCell.UpdateSelectedCue(cue);
                });
            }
        }

        public void OnCueUpdateReceived(object sender, CueEventArgs args)
        {
            if(qController != null)
            {
                if(qController.qWorkspace != null)
                {
                    qController.qWorkspace.UpdateCue(args.Cue);
                    if (qController.playbackPosition == null)
                    {
                        qController.playbackPosition = args.Cue.uniqueID;
                    }

                    if (args.Cue.uniqueID == qController.playbackPosition)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Log.Debug("CONTROLPAGE - Refreshing Currently Displayed Cue");
                            if (args.Cue.levels != null)
                                showLevelsButton.IsVisible = true;
                            else
                            {
                                showLevelsButton.IsVisible = false;
                                qLevelsCell.IsVisible = false;
                            }
                            qCell.UpdateSelectedCue(args.Cue);
                            if(qLevelsCell != null)
                            {
                                qLevelsCell.activeCue = args.Cue.uniqueID;
                                if (args.Cue.levels != null)
                                {
                                    qLevelsCell.UpdateLevels(args.Cue.levels[0]);
                                }
                            }
                        });
                    } 
                }
            }
            
        }

        private void OnChildrenUpdated(object source, ChildrenEventArgs args)
        {
            if(qController != null)
            {
                if(qController.qWorkspace != null)
                {
                    qController.qWorkspace.UpdateChildren(args.cue_id, args.children);  
                }
            }
        }

        private void OnMenuItemSelected(object source, MenuEventArgs args)
        {
            if (args.Command.Contains("/"))
            {
                qController.qClient.sendTCP(args.Command);
            }
            else if (args.Command == "disconnect")
            {
                Back();
            }else if (args.Command.Contains("cueList"))
            {
                var parts = args.Command.Split(' ');
                if(parts.Length > 1)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        qCueListCell = new QCueListCell(qController.qWorkspace.GetCueList(parts[1]));
                        qCueListCell.closeButton.Clicked += CloseCueList;
                        qCueListCell.cueListView.ItemSelected += OnCueListItemSelected;
                        AbsoluteLayout.SetLayoutFlags(qCueListCell, AbsoluteLayoutFlags.All);
                        AbsoluteLayout.SetLayoutBounds(qCueListCell, new Rectangle(0, 0.2, 1, 0.9));
                        sLayout.Children.Add(qCueListCell);
                    });
                }
            }
        }

        private void CloseCueList(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                sLayout.Children.Remove(qCueListCell);
            });
        }

        private void OnCueListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            OSCListItem cue = (OSCListItem)e.SelectedItem;
            string selectCueOSC = "/workspace/" + qController.qWorkspace.workspace_id + cue.Command;
            qController.qClient.sendTCP(selectCueOSC);
            CloseCueList(sender,e);
        }

    }
}
