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

            qController.qClient.sendAndReceiveString("/cueLists");
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

            Hashtable cmds = new Hashtable();

            var backButtonGesture = new TapGestureRecognizer();

            backButtonGesture.Tapped += Back;
            backButton.GestureRecognizers.Add(backButtonGesture);


            cmds.Add("Pause", "/pause");
            cmds.Add("Resume", "/resume");
            cmds.Add("Panic", "/panic");
            cmds.Add("Next", "/select/next");
            cmds.Add("Previous", "/select/previous");
            cmds.Add("Preview", "/preview");

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
            foreach (string cmd in cmds.Keys)
            {
                QButton b = new QButton(cmd,(string)cmds[cmd]);
                b.Clicked += sendOSC;
                if(cmd=="Panic")
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
            qController.sendCommand(((QButton)sender).OSCCommand);
        }

        void Back(object sender, EventArgs e)
        {
            App.iNav.PopModalAsync();
        }

        public void SelectedCueUpdated(object sender, CueEventArgs e){
            Device.BeginInvokeOnMainThread(()=>{
                if (e.SelectedCue.type.Equals("Audio"))
                {
                    //Console.WriteLine("CUE IS AN AUDIO CUE");
                    sLayout.Children.Add(qSelectedCueOptions);
                }
                else
                {
                    //Console.WriteLine("CUE IS NOT AN AUDIO CUE");
                    sLayout.Children.Remove(qSelectedCueOptions);
                }
                qCell.UpdateSelectedCue(e.SelectedCue);
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
                Console.WriteLine("Workspace Updated: " + e.UpdatedWorkspace.workspace_id);
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
