using Xamarin.Forms;

namespace qController.UI
{
    public class QNavBar : Frame
    {
        public Label menuButton;
        public Label instanceName;

        public QNavBar()
        {
            this.SetDynamicResource(Grid.BackgroundColorProperty, "NavigationBarColor");
            CornerRadius = 0;
            HasShadow = false;
            Padding = 0;
            Margin = 0;

            Grid grid = new Grid();
            grid.RowSpacing = 0;
            grid.ColumnSpacing = 0;
            grid.Margin = 0;
            grid.Padding = 0;
            

            grid.RowDefinitions = new RowDefinitionCollection {
                new RowDefinition { Height = 50 }
            };
            grid.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = new GridLength(4,GridUnitType.Star)},
            };

            menuButton = new Label
            {
                FontFamily = App.QFont,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 0,
                Margin = new Thickness(10, 0, 0, 0),
                Text = QIcon.MENU,
                BackgroundColor = Color.Transparent
            };

            menuButton.SetDynamicResource(Label.TextColorProperty, "IconTextColor");


            instanceName = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0),
                Padding = 0,
                FontSize = App.HeightUnit * 3,
                //BackgroundColor = Color.Blue
            };

            instanceName.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");


            var menuButtonGesture = new TapGestureRecognizer();
            menuButtonGesture.Tapped += App.ShowMenu;
            menuButton.GestureRecognizers.Add(menuButtonGesture);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    HeightRequest = App.HeightUnit * 6;
                    menuButton.FontSize = App.Height * .04;
                    break;
                case Device.Android:
                    HeightRequest = App.HeightUnit * 6;
                    menuButton.FontSize = App.Height * .04;
                    break;
            }

            grid.Children.Add(menuButton, 0, 0);
            grid.Children.Add(instanceName, 1, 0);
            Content = grid;
        }
    }
}
