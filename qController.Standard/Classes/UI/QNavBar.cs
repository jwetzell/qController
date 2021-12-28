using qController.Helpers;
using Xamarin.Forms;

namespace qController.UI
{
    public class QNavBar : Frame
    {
        public Label menuButton;
        public Label instanceName;

        public QNavBar()
        {
            SetDynamicResource(BackgroundColorProperty, "NavigationBarColor");
            CornerRadius = 0;
            HasShadow = false;

            Padding = 0;
            Margin = 0;

            Grid grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,

                Margin = 0,
                Padding = 0,

                RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition {Width = GridLength.Star},
                    new ColumnDefinition {Width = new GridLength(4,GridUnitType.Star)},
                },
            };

            menuButton = new Label
            {
                FontFamily = (OnPlatform<string>)Application.Current.Resources["MaterialFontFamily"],
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 0,
                Margin = new Thickness(10, 0, 0, 0),
                Text = IconConstants.Menu,
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
            };

            instanceName.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");


            TapGestureRecognizer menuButtonGesture = new TapGestureRecognizer();
            menuButtonGesture.Tapped += App.ShowMenu;
            menuButton.GestureRecognizers.Add(menuButtonGesture);

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    HeightRequest = App.HeightUnit * 6;
                    menuButton.HeightRequest = App.HeightUnit * 6;
                    instanceName.HeightRequest = App.HeightUnit * 6;
                    menuButton.FontSize = App.HeightUnit * 5;
                    break;
                case Device.Android:
                    HeightRequest = App.HeightUnit * 6;
                    menuButton.HeightRequest = App.HeightUnit * 6;
                    instanceName.HeightRequest = App.HeightUnit * 6;
                    menuButton.FontSize = App.HeightUnit * 5;
                    break;
                default:
                    HeightRequest = App.HeightUnit * 6;
                    menuButton.HeightRequest = App.HeightUnit * 6;
                    instanceName.HeightRequest = App.HeightUnit * 6;
                    menuButton.FontSize = App.HeightUnit * 5;
                    break;
            }

            grid.Children.Add(menuButton, 0, 0);
            grid.Children.Add(instanceName, 1, 0);
            Content = grid;
        }
    }
}
