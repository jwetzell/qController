using Xamarin.Forms;
namespace qController.UI.Buttons
{
    public class QLevelsButton : ShadowButton
    {
        public Button button;
        public QLevelsButton()
        {
            button = new Button
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

            HeightRequest = button.Height;
            WidthRequest = button.Width;
            CornerRadius = button.CornerRadius;
            Content = button;
        }
    }
}
