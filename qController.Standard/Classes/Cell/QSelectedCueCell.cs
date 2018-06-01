using System;
using Xamarin.Forms;

namespace qController
{
    public class QSelectedCueCell : Frame
    {
        Label name;
        Label number;
        Label type;
        Label notes;

        public QSelectedCueCell()
        {
            Padding = new Thickness(5);
            CornerRadius = 20;
            BackgroundColor = Color.FromHex("D8D8D8");
            Grid mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };
            number = new Label { Text = "N/A"};
            number.FontAttributes = FontAttributes.Bold;
            number.FontSize = 30;
            number.Margin = new Thickness(5);


            name = new Label { Text = "N/A" };
            name.FontAttributes = FontAttributes.Bold;
            name.HorizontalTextAlignment = TextAlignment.Center;
            name.FontSize = 20;
            name.Margin = new Thickness(0);

            type = new Label { Text = "N/A" };
            type.FontAttributes = FontAttributes.Bold;
            type.VerticalTextAlignment = TextAlignment.Center;
            type.HorizontalTextAlignment = TextAlignment.Center;
            type.Margin = new Thickness(5);

            notes = new Label { Text = "Viewing Notes Currently Unavailable" };
            notes.HorizontalTextAlignment = TextAlignment.Center;
            notes.Margin = new Thickness(0, 0, 0, 10);

            mainG.Children.Add(number, 0, 0);
            mainG.Children.Add(type, 4, 0);
            mainG.Children.Add(name, 0, 1);
            mainG.Children.Add(notes, 0, 2);
            Grid.SetColumnSpan(name, 5);
            Grid.SetColumnSpan(notes,5);


            Margin = new Thickness(10);
            Content = mainG;
        }

        public void UpdateSelectedCue(QCue cue)
        {
            name.Text = cue.name;
            number.Text = cue.number;
            type.Text = cue.type;
            //notes.Text = cue.notes;
        }
    }
}
