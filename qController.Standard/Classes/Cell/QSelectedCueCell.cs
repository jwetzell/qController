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
                RowDefinitions = 
                {
                    new RowDefinition{Height = GridLength.Star},
                    new RowDefinition{Height = GridLength.Star}
                },
                ColumnDefinitions = 
                {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };
            Grid topGrid = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = 
                {
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };
            Grid bottomGrid = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions =
                {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };

            number = new Label { 
                Text = "N/A",
                FontAttributes = FontAttributes.Bold,
                FontSize = 30,
                Margin = new Thickness(5)
            };

            name = new Label { 
                Text = "N/A",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 20,
                Margin = new Thickness(0)
            };

            type = new Label { 
                Text = "", 
                FontFamily = App.QFont,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.End,
                Margin = new Thickness(5),
                FontSize = 27
            };

            notes = new Label { Text = "Viewing Notes Currently Unavailable" };
            notes.HorizontalTextAlignment = TextAlignment.Center;
            notes.Margin = new Thickness(0, 0, 0, 10);

            topGrid.Children.Add(number, 0, 0);
            topGrid.Children.Add(type, 4, 0);
            Grid.SetColumnSpan(number, 2);
            Grid.SetColumnSpan(type, 3);

            bottomGrid.Children.Add(name, 0, 0);
            bottomGrid.Children.Add(notes, 0, 1);
            Grid.SetColumnSpan(name, 5);
            Grid.SetColumnSpan(notes,5);

            mainG.Children.Add(topGrid, 0, 0);
            Grid.SetColumnSpan(topGrid, 5);

            mainG.Children.Add(bottomGrid, 0, 1);
            Grid.SetColumnSpan(bottomGrid, 5);

            Margin = new Thickness(10);
            Content = mainG;
        }

        public void UpdateSelectedCue(QCue cue)
        {
            name.Text = cue.listName;
            number.Text = cue.number;
            type.Text = cue.getIconString();
            notes.Text = cue.notes;
        }
    }
}
