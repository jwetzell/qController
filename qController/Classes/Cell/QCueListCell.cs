using System.Collections.ObjectModel;
using qController.QItems;

namespace qController.Cell
{
    public class QCueListCell : Frame
    {
        public ListView cueListView;
        public Button closeButton;
        public ObservableCollection<OSCListItem> items;
        public QCueListCell(QCueList qCueList)
        {
            Margin = new Thickness(10, 10, 10, 10);
            Padding = new Thickness(10);
            items = new ObservableCollection<OSCListItem>();

            cueListView = new ListView
            {
                ItemsSource = items,
                RowHeight = (int)(App.HeightUnit * 6),
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid { Padding = new Thickness(5, 10) };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });


                    var icon = new Label();

                    icon.FontFamily = App.QFont;
                    icon.SetBinding(Label.TextProperty, "Icon");
                    icon.HorizontalTextAlignment = TextAlignment.Center;
                    icon.VerticalTextAlignment = TextAlignment.Center;
                    icon.FontSize = App.HeightUnit * 3;
                    icon.TextColor = Colors.Black;
                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Text");
                    if (label.Text == "Disconnect")
                    {
                        label.TextColor = Colors.DarkRed;
                    }
                    label.FontSize = App.HeightUnit * 2.2;
                    label.TextColor = Colors.Black;
                    grid.Add(icon);
                    grid.Add(label, 1, 0);
                    return new ViewCell { View = grid };

                })
            };
            StackLayout layout = new StackLayout();
            closeButton = new Button
            {
                BackgroundColor = Colors.Gray,
                Text = "Close",
                TextColor = Colors.Black
            };

            layout.Children.Add(closeButton);
            layout.Children.Add(cueListView);
            Content = layout;

            for(int j=0; j < qCueList.cues.Count; j++)
            {
                var cue = qCueList.cues[j];
                AddSubCues(cue, 0);
            }

        }

        public void AddSubCues(QCue cue, int level)
        {
            var cueIcon = cue.getIconString();
            var cueTitle = "";
            for (int i = 0; i < level; i++)
            {
                cueTitle += "   ";
            }
            if (cue.number != "")
            {
                cueTitle += cue.number + " - " + cue.listName;
            }
            else
            {
                cueTitle += cue.listName;
            }

            if (cue.cues != null)
            {
                items.Add(new OSCListItem
                {
                    Text = cueTitle,
                    Icon = cueIcon,
                    Command = "/select_id/" + cue.uniqueID
                });

                //uncomment to load nested group cues
                for (int i = 0; i < cue.cues.Count; i++)
                {
                    var sub_cue = cue.cues[i];
                    AddSubCues(sub_cue, level + 1);
                }
            }
            else
            {
                items.Add(new OSCListItem
                {
                    Text = cueTitle,
                    Icon = cueIcon,
                    Command = "/select_id/" + cue.uniqueID
                });
            }
        }

    }
}
