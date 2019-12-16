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
        QLevelsCell qLevelsCell;
        QCueListCell qCueListCell;
        ShadowButton showLevelsButton;
        QControlsBlock qControlsBlock;
        public ControlPage(string name, string address)
        {
            InitializeComponent();

            qController = new QController(address, 53000);
            qController.qClient.qParser.WorkspaceInfoReceived += WorkspaceInfoReceived;
            qController.qClient.qParser.WorkspaceUpdated += WorkspaceUpdated;
            qController.qClient.qParser.WorkspaceDisconnect += WorkspaceDisconnected;
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
                Console.WriteLine("CONTROLPAGE - MULTIPLE WORKSPACES ON SELECTED COMPUTER");
                PromptForWorkspace(args.WorkspaceInfo);
            }
            else if (args.WorkspaceInfo.Count == 1)
            {
                Console.WriteLine("CONTROLPAGE - ONLY ONE WORKSPACE ON SELECTED COMPUTER");
                if (!args.WorkspaceInfo[0].hasPasscode)
                {
                    qController.Connect(args.WorkspaceInfo[0].uniqueID);
                    Device.BeginInvokeOnMainThread(() => {
                        FinishUI();
                    });
                }
                else
                {
                    UserDialogs.Instance.Confirm(new ConfirmConfig
                    {
                        Message = "Woops....I haven't implemented password protected workspaces yet...",
                        OkText = "Disconnect",
                        OnAction = (resp) =>
                        {
                            if (resp)
                                Back();
                        }
                    });
                }
                
            }
            else
            {
                UserDialogs.Instance.Confirm(new ConfirmConfig
                {
                    Message = "QLab doesn't have any workspaces open?",
                    OkText = "Disconnect",
                    OnAction = (resp) =>
                    {
                        if (resp)
                            Back();
                    }
                });
            }
        }

        private void PromptForWorkspace(List<QWorkspaceInfo> workspaces)
        {
            ActionSheetConfig config = new ActionSheetConfig();
            config.SetTitle("Select Workspace");
            for (int i = 0; i < workspaces.Count; i++)
            {
                QWorkspaceInfo workspace = workspaces[i];
                config.Add(workspace.displayName, new Action(() => {
                    Console.WriteLine("CONTROLPAGE - Workspace Selected " + workspace.displayName);
                    if (!workspace.hasPasscode)
                    {
                        qController.Connect(workspace.uniqueID);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FinishUI();
                        });
                    }
                    else
                    {
                        UserDialogs.Instance.Confirm(new ConfirmConfig
                        {
                            Message = "Woops....I haven't implemented password protected workspaces yet...",
                            OkText = "Disconnect",
                            OnAction = (resp) =>
                            {
                                if (resp)
                                    Back();
                            }
                        });
                    }
                    
                }));
            }
            UserDialogs.Instance.ActionSheet(config);
        }

        private void InitGUI()
        {
            App.rootPage.MenuPage.ChangeToControl();
            NavigationPage.SetHasNavigationBar(this, false);

            BackgroundColor = Color.FromHex("4A4A4A");
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

        private void OnSelectedCueEdited(object source, CueEditArgs args)
        {
            string address = "/workspace/" + qController.qWorkspace.workspace_id + "/cue_id/" + args.CueID + "/" + args.Property;
            qController.qClient.sendArgsUDP(address, args.NewValue);
        }

        void FinishUI()
        {
            string workspace_prefix = "/workspace/" + qController.qWorkspace.workspace_id;
            
            qLevelsCell = new QLevelsCell();

            qLevelsCell.mainSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs(workspace_prefix + "/cue_id/" + qLevelsCell.activeCue + "/sliderLevel/0", (float)args.NewValue);
            };
            qLevelsCell.leftSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs(workspace_prefix + "/cue_id/" + qLevelsCell.activeCue + "/sliderLevel/1", (float)args.NewValue);
            };
            qLevelsCell.rightSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs(workspace_prefix + "/cue_id/" + qLevelsCell.activeCue + "/sliderLevel/2", (float)args.NewValue);
            };

            Button showLevelsInnerButton = new Button
            {
                Text = QIcon.SLIDERS,
                TextColor = Color.Black,
                FontFamily = App.QFont,
                FontSize = App.HeightUnit * 4,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = App.HeightUnit * 8,
                WidthRequest = App.HeightUnit * 8,
                CornerRadius = (int)(App.HeightUnit * 4),
                BackgroundColor = Color.LightBlue
            };
            showLevelsInnerButton.Clicked += (s, e) =>
            {
                qLevelsCell.IsVisible = !qLevelsCell.IsVisible;
            };

            showLevelsButton = new ShadowButton(showLevelsInnerButton);
            qControlsBlock = new QControlsBlock(qController);

            AbsoluteLayout.SetLayoutFlags(qControlsBlock, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutFlags(qLevelsCell, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutFlags(showLevelsButton, AbsoluteLayoutFlags.PositionProportional);


            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    AbsoluteLayout.SetLayoutBounds(qControlsBlock, new Rectangle(0, 0.53, 1, 0.25));
                    AbsoluteLayout.SetLayoutBounds(qLevelsCell, new Rectangle(0.9,0.40,0.8,0.25));
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

        void ShowMenu(object sender, EventArgs e)
        {
            App.MenuIsPresented = true;
        }

        void Back()
        {
            qController.Kill();
            App.rootPage.MenuItemSelected -= OnMenuItemSelected;
            Device.BeginInvokeOnMainThread(() =>
            {
                App.rootPage.MenuPage.ChangeToHome();
                App.NavigationPage.Navigation.PopAsync();
            });
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
                            QCue noSelect = new QCue();
                            noSelect.listName = "No Cue Selected";
                            noSelect.type = "";
                            noSelect.notes = "Workspace has loaded but no cue is selected";
                            noSelect.number = "!";
                            qCell.UpdateSelectedCue(noSelect);
                        });
                        Console.WriteLine("CONTROLPAGE - Update Selected Cue Called because of Inital Workspace Load");
                        qController.qClient.UpdateSelectedCue(qController.qWorkspace.workspace_id);
                    }
                    Console.WriteLine("CONTROLPAGE - Workspace Updated " + qController.qWorkspace.workspace_id);
                }
            }
        }

        public void PlaybackPositionUpdated(object sender, PlaybackPositionArgs e)
        {
            qController.playbackPosition = e.PlaybackPosition;
            Console.WriteLine("CONTROLPAGE - Playback Position Updated " + qController.playbackPosition);
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
                            Console.WriteLine("CONTROLPAGE - Refreshing Currently Displayed Cue");
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
                qController.qClient.sendStringUDP(args.Command);
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
            CloseCueList();
        }

        private void CloseCueList()
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
            qController.qClient.sendStringUDP(selectCueOSC);
            CloseCueList();
        }

        protected override bool OnBackButtonPressed()
        {
            qController.Kill();
            App.rootPage.MenuItemSelected -= OnMenuItemSelected;
            Device.BeginInvokeOnMainThread(() =>
            {
                App.rootPage.MenuPage.ChangeToHome();
            });
            return base.OnBackButtonPressed();
        }
    }
}
