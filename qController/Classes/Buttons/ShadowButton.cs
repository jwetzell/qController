using Microsoft.Maui.Graphics;
namespace qController
{
    public class ShadowButton : Frame
    {
        public ShadowButton(Button b)
        {
            //Content = b;
            HeightRequest = b.Height;
            WidthRequest = b.Width;
            CornerRadius = b.CornerRadius;
            Padding = new Thickness(0);
            Content = b;
            HasShadow = true;
            BorderColor = Colors.Black;
            IsVisible = false;
        }

        public ShadowButton()
        {
            HasShadow = true;
            BorderColor = Colors.Black;
            IsVisible = false;
            Padding = new Thickness(0);
        }
    }
}
