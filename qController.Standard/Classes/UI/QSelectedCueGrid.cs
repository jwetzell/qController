using Acr.UserDialogs;
using Xamarin.Forms;

namespace qController.UI
{
    public class QSelectedCueGrid : Grid
    {
        public QSelectedCueGrid()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;
            BackgroundColor = Color.FromHex("#D8D8D8");

            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            Frame border = new Frame
            {
                BorderColor = Color.Black,
                BackgroundColor = Color.Transparent
            };

            Children.Add(border);

            SetRowSpan(border, 3);
            SetColumnSpan(border, 5);

            Label number = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = 0,
                Padding = 0,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            number.SetBinding(Label.TextProperty, "number", BindingMode.OneWay);
            Children.Add(number,0,0);

            Label type = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = 0,
                Padding = 0,
                FontSize = App.HeightUnit * 4,
                FontFamily = App.QFont
            };
            type.SetBinding(Label.TextProperty, "type", BindingMode.OneWay);
            Children.Add(type, 4, 0);


            Label name = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            name.SetBinding(Label.TextProperty, "name", BindingMode.OneWay);
            Children.Add(name, 0, 1);
            SetColumnSpan(name, 5);

            Editor notes = new Editor
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = 5,
                BackgroundColor = Color.Transparent,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Editor))
            };
            notes.SetBinding(Editor.TextProperty, "notes", BindingMode.TwoWay);

            Children.Add(notes, 0, 2);
            SetColumnSpan(notes, 5);

        }
    }
}
