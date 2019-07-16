using System;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace qController
{
    public class CueEditArgs : EventArgs
    {
        public string CueID
        {
            get;
            set;
        }
        public string Property
        {
            get;
            set;
        }
        public string NewValue
        {
            get;
            set;
        }
    }

    public class QSelectedCueCell : Frame
    {
        public delegate void SelectedCueEditedHandler(object source, CueEditArgs args);
        public event SelectedCueEditedHandler SelectedCueEdited;

        Label name;
        Label number;
        Label type;
        public Label notes;
        public QCue activeCue;
        public QSelectedCueCell()
        {
        
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
                Margin = new Thickness(0,0,0,10),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            
            //BACKGROUND COLORS FOR TESTING ONLY 
            //notes.BackgroundColor = Color.Red;
            //number.BackgroundColor = Color.Red;
            //name.BackgroundColor = Color.Red;
            //type.BackgroundColor = Color.Red;
            //topGrid.BackgroundColor = Color.Green;
            //bottomGrid.BackgroundColor = Color.Green;

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

            CornerRadius = 20;
            BackgroundColor = Color.FromHex("D8D8D8");
            HeightRequest = App.HeightUnit * 25;
            Margin = new Thickness(10);
            Content = mainG;

            SetupDoubleTapEdit();
        }

        void SetupDoubleTapEdit()
        {
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
                        OnSelectedCueEdited("notes", qNotes.Text);


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
                        OnSelectedCueEdited("name", qName.Text);
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
                        OnSelectedCueEdited("number", qNumber.Text);
                    }
                });
            };
            notes.GestureRecognizers.Add(notesDoubleTap);
            name.GestureRecognizers.Add(nameDoubleTap);
            number.GestureRecognizers.Add(numberDoubleTap);
        }

        public void UpdateSelectedCue(QCue cue)
        {
            activeCue = cue;
            name.Text = cue.listName;
            number.Text = cue.number;
            type.Text = cue.getIconString();
            notes.Text = cue.notes;
        }

        protected virtual void OnSelectedCueEdited(string prop, string value)
        {
            if (SelectedCueEdited != null)
                SelectedCueEdited(this, new CueEditArgs() { CueID = activeCue.uniqueID, Property = prop, NewValue = value }) ;

        }
    }
}
