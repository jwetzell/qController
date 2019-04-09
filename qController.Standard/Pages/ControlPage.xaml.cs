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
            qController.qClient.qParser.SelectedCueUpdated += SelectedCueUpdated;
            qController.qClient.qParser.WorkspaceUpdated += WorkspaceUpdated;
            qController.qClient.qParser.PlaybackPositionUpdated += PlaybackPositionUpdated;
            qController.qClient.qParser.CueInfoUpdated += OnCueUpdateReceived;
            NavigationPage.SetHasNavigationBar(this, false);
            instanceName.Text = name;


            InitGUI();

        }

        private void InitGUI()
        {


            instanceName.HorizontalTextAlignment = TextAlignment.Center;
            instanceName.HorizontalOptions = LayoutOptions.CenterAndExpand;
            instanceName.VerticalOptions = LayoutOptions.CenterAndExpand;
            topBar.Children.Add(instanceName, 1, 0);
            topBar.BackgroundColor = Color.FromHex("71AEFF");

            BackgroundColor = Color.FromHex("4A4A4A");
            backButton.Source = ImageSource.FromFile("back");
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    backButton.Margin = new Thickness(20, 45, 20, 10);
                    instanceName.Margin = new Thickness(20, 45, 20, 10);
                    break;

            }

            List<QButton> buttons = new List<QButton>();

            buttons.Add(new QButton("Previous", "/select/previous"));
            buttons.Add(new QButton("Panic", "/panic"));
            buttons.Add(new QButton("Next", "/select/next"));
            buttons.Add(new QButton("Preview", "/preview"));
            buttons.Add(new QButton("Pause", "/pause"));
            buttons.Add(new QButton("Resume", "/resume"));

            var backButtonGesture = new TapGestureRecognizer();

            backButtonGesture.Tapped += Back;
            backButton.GestureRecognizers.Add(backButtonGesture);


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
                }
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

        void Back(object sender, EventArgs e)
        {
            App.iNav.PopModalAsync();
        }

        public void SelectedCueUpdated(object sender, CueEventArgs e){
            Device.BeginInvokeOnMainThread(()=>{
                if (e.Cue.type.Equals("Audio"))
                {
                    //Console.WriteLine("CUE IS AN AUDIO CUE");
                    sLayout.Children.Add(qSelectedCueOptions);
                }
                else
                {
                    //Console.WriteLine("CUE IS NOT AN AUDIO CUE");
                    sLayout.Children.Remove(qSelectedCueOptions);
                }
                qCell.UpdateSelectedCue(e.Cue);
            });
        }

        public void WorkspaceUpdated(object sender, WorkspaceEventArgs e)
        {
            if(e.UpdatedWorkspace.data.Count > 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    //
                });
                qController.qWorkspace = e.UpdatedWorkspace;
                Console.WriteLine("Workspace updated in ControlPage: " + qController.qWorkspace.workspace_id);
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
                    {
                        //Console.WriteLine("CUE IS AN AUDIO CUE");
                        //sLayout.Children.Add(qSelectedCueOptions);
                    }
                    else
                    {
                        //Console.WriteLine("CUE IS NOT AN AUDIO CUE");
                        //sLayout.Children.Remove(qSelectedCueOptions);
                    }
                    qCell.UpdateSelectedCue(cue);
                });
            }
        }

        public void OnCueUpdateReceived(object sender, CueEventArgs args)
        {
            qController.qWorkspace.UpdateCue(args.Cue);
            Console.WriteLine("Cue updated in ControlPage: " + args.Cue.uniqueID);
            if (args.Cue.uniqueID == qController.playbackPosition)
            {
                Console.WriteLine("Cue that was updated is equal to the playback position.");
                Device.BeginInvokeOnMainThread(() =>
                {
                    qCell.UpdateSelectedCue(args.Cue);
                });
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
