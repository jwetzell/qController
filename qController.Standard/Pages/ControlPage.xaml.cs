using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections;

namespace qController
{
    public partial class ControlPage : ContentPage
    {
        QController qController;
        Label instanceName = new Label();
        QSelectedCueCell qCell;
        QSelectedCueOptionsCell qSelectedCueOptions;
        Grid mainG;

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


        }

        private void WorkspaceInfoReceived(object source, WorkspaceInfoArgs args)
        {
            if(args.WorkspaceInfo.Count > 1)
            {
                Console.WriteLine("MULTIPLE WORKSPACES ON SELECTED COMPUTER");
            }
            else
            {
                Console.WriteLine("ONLY ONE WORKSPACE ON SELECTED COMPUTER");
                //qController.Connect();
            }
        }

        private void InitGUI()
        {

            NavigationPage.SetHasNavigationBar(this, false);
            instanceName.HorizontalTextAlignment = TextAlignment.Center;
            instanceName.HorizontalOptions = LayoutOptions.CenterAndExpand;
            instanceName.VerticalOptions = LayoutOptions.CenterAndExpand;
            topBar.Children.Add(instanceName, 1, 0);
            topBar.BackgroundColor = Color.FromHex("71AEFF");

            BackgroundColor = Color.FromHex("4A4A4A");
            menuButton.Text = QIcon.MENU;
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    menuButton.Margin = new Thickness(20, 35, 20, 10);
                    instanceName.Margin = new Thickness(20, 35, 20, 10);
                    break;

            }

            List<QButton> buttons = new List<QButton>();

            buttons.Add(new QButton("Previous", "/select/previous"));
            buttons.Add(new QButton("Panic", "/panic"));
            buttons.Add(new QButton("Next", "/select/next"));
            buttons.Add(new QButton("Preview", "/preview"));
            buttons.Add(new QButton("Pause", "/pause"));
            buttons.Add(new QButton("Resume", "/resume"));

            var menuButtonGesture = new TapGestureRecognizer();

            menuButtonGesture.Tapped += ShowMenu;
            menuButton.GestureRecognizers.Add(menuButtonGesture);


            qCell = new QSelectedCueCell();
            qSelectedCueOptions = new QSelectedCueOptionsCell(qController);

            sLayout.Children.Add(qCell);


            mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                },
                Margin = new Thickness(10),
                IsVisible = false
            };


            int row = 0;
            int column = 0;
            foreach (var b in buttons)
            {
                b.Clicked += sendOSC;
                if(b.Text=="Panic")
                {
                    b.BackgroundColor = Color.IndianRed;
                }
                else
                {
                    b.BackgroundColor = Color.FromHex("D8D8D8");
                }
                b.TextColor = Color.Black;
                mainG.Children.Add(b,column,row);
                row++;
                if(row == 3){
                    row = 0;
                    column = 2;
                }
            }

            QButton goButton = new QButton("GO","/go");
            goButton.Clicked += sendOSC;
            goButton.BackgroundColor = Color.SeaGreen;
            goButton.TextColor = Color.Black;
            mainG.Children.Add(goButton,1,0);
            Grid.SetRowSpan(goButton,3);

            sLayout.Children.Add(mainG);

        }

        void sendOSC(object sender, EventArgs e)
        {
            qController.qClient.sendStringUDP(((QButton)sender).OSCCommand);
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
                    if (!mainG.IsVisible)
                        mainG.IsVisible = true;
                });
            }
        }

        private void OnChildrenUpdated(object source, ChildrenEventArgs args)
        {
            Console.WriteLine("Children Updated in ControlPage: " + args.cue_id);
            qController.qWorkspace.UpdateChildren(args.cue_id, args.children);
            if (qController.qWorkspace.IsPopulated)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (App.MenuIsPresented)
                    {
                        App.rootPage.MenuPage.ChangeToWorkspace(qController.qWorkspace);
                    }
                    else
                    {
                        App.rootPage.MenuPage.ChangeToWorkspace(qController.qWorkspace);

                    }
                });
                qController.qClient.UpdateSelectedCue();
                App.showToast("Workspace cues loaded...");
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
