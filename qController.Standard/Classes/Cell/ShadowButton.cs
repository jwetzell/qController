using Xamarin.Forms;

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
            BorderColor = Color.Black;
            IsVisible = false;
        }
    }
}
