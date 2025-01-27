using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
namespace qController
{
    public class QLevelsButton : ShadowButton
    {
        public Button button;
        public QLevelsButton()
        {
            button = new Button
            {
                Text = QIcon.SLIDERS,
                TextColor = Colors.Black,
                FontFamily = App.QFont,
                FontSize = App.HeightUnit * 4,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = App.HeightUnit * 8,
                WidthRequest = App.HeightUnit * 8,
                CornerRadius = (int)(App.HeightUnit * 4),
                BackgroundColor = Colors.LightBlue
            };

            HeightRequest = button.Height;
            WidthRequest = button.Width;
            CornerRadius = button.CornerRadius;
            Content = button;
        }
    }
}
