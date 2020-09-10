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
            number.SetBinding(Label.TextProperty, "number", BindingMode.TwoWay);
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
            name.SetBinding(Label.TextProperty, "name", BindingMode.TwoWay);
            Children.Add(name, 0, 1);
            SetColumnSpan(name, 5);


            Label notes = new Label
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            notes.SetBinding(Label.TextProperty, "notes", BindingMode.TwoWay);
            Children.Add(notes, 0, 2);
            SetColumnSpan(notes, 5);


            //Double Tap to Edit
            var notesDoubleTap = new TapGestureRecognizer();
            var nameDoubleTap = new TapGestureRecognizer();
            var numberDoubleTap = new TapGestureRecognizer();

            notesDoubleTap.NumberOfTapsRequired = 2;
            nameDoubleTap.NumberOfTapsRequired = 2;
            numberDoubleTap.NumberOfTapsRequired = 2;

            notesDoubleTap.Tapped += (s, e) =>
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    Title = "Update Notes",
                    Message = "Changes notes to update",
                    OkText = "Update",
                    Text = notes.Text,
                    OnAction = (qNotes) =>
                    {
                        if (!qNotes.Ok)
                            return;
                        notes.Text = qNotes.Text;
                    }
                });
            };

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
                        number.Text = qNumber.Text;
                    }
                });
            };
            notes.GestureRecognizers.Add(notesDoubleTap);
            name.GestureRecognizers.Add(nameDoubleTap);
            number.GestureRecognizers.Add(numberDoubleTap);
        }
    }
}
