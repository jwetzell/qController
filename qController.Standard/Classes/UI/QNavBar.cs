using Xamarin.Forms;

namespace qController.UI
{
    public class QNavBar : Grid
    {
        public Button menuButton;
        public Label instanceName;

        public QNavBar()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;
            Padding = 0;
            Margin = 0;
            this.SetDynamicResource(Grid.BackgroundColorProperty, "NavigationBarColor");
            RowDefinitions =  new RowDefinitionCollection {
                new RowDefinition { Height = GridLength.Star }
            };
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = new GridLength(4,GridUnitType.Star)},
            };

            menuButton = new Button
            {
                FontFamily = App.QFont,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Margin = new Thickness(10,0,0,0),
                Text = QIcon.MENU,
                CornerRadius = 0,
                //BackgroundColor = Color.Red
            };

            menuButton.SetDynamicResource(Label.TextColorProperty, "IconTextColor");
            

            instanceName = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0,0,10,0),
                Padding = 0,
                FontSize = App.HeightUnit * 3,
                //BackgroundColor = Color.Blue
            };

            instanceName.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");


            menuButton.Clicked += App.ShowMenu;

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

            this.Children.Add(menuButton, 0, 0);
            this.Children.Add(instanceName, 1, 0);
        }
    }
}
