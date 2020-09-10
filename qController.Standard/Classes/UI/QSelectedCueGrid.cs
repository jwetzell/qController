using Acr.UserDialogs;
using qController.ViewModels;
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
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
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
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Transparent,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            name.SetBinding(Label.TextProperty, "name", BindingMode.TwoWay);
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

            //Double tap to edit name/number

            var nameDoubleTap = new TapGestureRecognizer();
            nameDoubleTap.NumberOfTapsRequired = 2;

            nameDoubleTap.Tapped += (s, e) =>
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    Title = "Update Name",
                    Message = "Change name to update",
                    OkText = "Update",
                    Text = name.Text,
                    OnAction = (qName) =>
                    {
                        if (!qName.Ok)
                            return;
                        name.Text = qName.Text;
                    }
                });
            };

            name.GestureRecognizers.Add(nameDoubleTap);

            var numberDoubleTap = new TapGestureRecognizer();
            numberDoubleTap.NumberOfTapsRequired = 2;

            numberDoubleTap.Tapped += (s, e) =>
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    Title = "Update Number",
                    Message = "Change number to update",
                    OkText = "Update",
                    Text = number.Text,
                    OnAction = (qNumber) =>
                    {
                        if (!qNumber.Ok)
                            return;
                        //This bypasses the label .Text property because the number might not be valid (no duplicates)
                        ((QCueViewModel)BindingContext).number = qNumber.Text;
                    }
                });
            };

            number.GestureRecognizers.Add(numberDoubleTap);


        }
    }
}
