using Xamarin.Forms;

namespace qController.UI.Buttons
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
            BorderColor = Color.Black;
            IsVisible = false;
        }

        public ShadowButton()
        {
            HasShadow = true;
            BorderColor = Color.Black;
            IsVisible = false;
            Padding = new Thickness(0);
        }
    }
}
