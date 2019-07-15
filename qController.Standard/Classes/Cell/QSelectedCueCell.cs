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
        public QCue activeCue;
        public QSelectedCueCell()
        {
            Padding = new Thickness(5);
            CornerRadius = 20;
            BackgroundColor = Color.FromHex("D8D8D8");
            HeightRequest = App.HeightUnit * 25;
            Grid mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = 
                {
                    new RowDefinition{Height = GridLength.Star},
                    new RowDefinition{Height = new GridLength(2, GridUnitType.Star)}
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
                    new RowDefinition{Height = new GridLength(1,GridUnitType.Star)},
                    new RowDefinition{Height = new GridLength(2,GridUnitType.Star)}
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
                Text = "",
                FontAttributes = FontAttributes.Bold,
                FontSize = App.HeightUnit * 5
            };

            name = new Label { 
                Text = "Loading Workspace....",
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = App.HeightUnit * 3.5,
                Margin = new Thickness(0)
            };

            type = new Label { 
                Text = QIcon.SPIN3, 
                FontFamily = App.QFont,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.End,
                FontSize = App.HeightUnit * 5
            };

            notes = new Label { 
                Text = "Loading Cue Lists and Playhead Position",
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0,0,0,10)
            };
            notes.VerticalOptions = LayoutOptions.FillAndExpand;
            notes.HorizontalOptions = LayoutOptions.FillAndExpand;


            notes.BackgroundColor = Color.FromHex("FF0000");
            number.BackgroundColor = Color.Red;
            name.BackgroundColor = Color.Red;
            type.BackgroundColor = Color.Red;

            var notesDoubleTap = new TapGestureRecognizer();
            notesDoubleTap.NumberOfTapsRequired = 2;
            notesDoubleTap.Tapped += (s, e) =>
            {
                Console.WriteLine("Notes double tapped");
            };

            number.GestureRecognizers.Add(notesDoubleTap);
            type.GestureRecognizers.Add(notesDoubleTap);
            name.GestureRecognizers.Add(notesDoubleTap);
            notes.GestureRecognizers.Add(notesDoubleTap);

            topGrid.Children.Add(number, 0, 0);
            topGrid.Children.Add(type, 4, 0);
            Grid.SetColumnSpan(number, 3);
            Grid.SetColumnSpan(type, 1);

            bottomGrid.Children.Add(name, 0, 0);
            bottomGrid.Children.Add(notes, 0, 1);
            Grid.SetColumnSpan(name, 5);
            Grid.SetColumnSpan(notes,5);

            mainG.Children.Add(topGrid, 0, 0);
            Grid.SetColumnSpan(topGrid, 5);

            mainG.Children.Add(bottomGrid, 0, 1);
            Grid.SetColumnSpan(bottomGrid, 5);

            topGrid.BackgroundColor = Color.FromHex("00FF00");
            bottomGrid.BackgroundColor = Color.FromHex("00FF00");
            Margin = new Thickness(10);
            Content = mainG;
        }

        public void UpdateSelectedCue(QCue cue)
        {
            activeCue = cue;
            name.Text = cue.listName;
            number.Text = cue.number;
            type.Text = cue.getIconString();
            notes.Text = cue.notes;
            if(notes.Text == ""){
                notes.Text = " ";
            }
        }
    }
}
