using System.Collections.ObjectModel;
using qController.QItems;
using Microsoft.Maui.Controls.Compatibility;

namespace qController.Cell
{
    public class QCueListCell : Frame
    {
        public ListView cueListView;
        public Button closeButton;
        public ObservableCollection<OSCListItem> items;
        public QCueListCell(QCueList qCueList)
        {
            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    Margin = new Thickness(10, 30, 10, 10);
                    break;
                case Device.Android:
                    Margin = new Thickness(10, 10, 10, 10);
                    break;
            }
            Padding = new Thickness(10);
            items = new ObservableCollection<OSCListItem>();

            cueListView = new ListView
            {
                ItemsSource = items,
                RowHeight = (int)(App.HeightUnit * 6),
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Microsoft.Maui.Controls.Compatibility.Grid { Padding = new Thickness(5, 10) };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });


                    var icon = new Label();

                    icon.FontFamily = App.QFont;
                    icon.SetBinding(Label.TextProperty, "Icon");
                    icon.HorizontalTextAlignment = TextAlignment.Center;
                    icon.VerticalTextAlignment = TextAlignment.Center;
                    icon.FontSize = App.HeightUnit * 3;
                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Text");
                    if (label.Text == "Disconnect")
                    {
                        label.TextColor = Colors.DarkRed;
                    }
                    // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            label.FontSize = App.HeightUnit * 3;
                            break;
                        case Device.Android:
                            label.FontSize = App.HeightUnit * 2.2;
                            break;
                    }
                    grid.Children.Add(icon);
                    grid.Children.Add(label, 1, 0);
                    return new ViewCell { View = grid };

                })
            };
            Microsoft.Maui.Controls.Compatibility.StackLayout layout = new Microsoft.Maui.Controls.Compatibility.StackLayout();
            closeButton = new Button
            {
                BackgroundColor = Colors.Gray,
                Text = "Close",
                TextColor = Colors.White
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
