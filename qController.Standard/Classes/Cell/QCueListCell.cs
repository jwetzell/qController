using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace qController
{
    public class QCueListCell : Frame
    {
        public ListView cueListView;
        public Button closeButton;
        public QCueListCell(QCueList qCueList)
        {
            Margin = new Thickness(10);
            Padding = new Thickness(10);


            cueListView = new ListView
            {
                ItemsSource = qCueList.cues,
                RowHeight = (int)(App.HeightUnit * 6),
                ItemTemplate = new DataTemplate(() =>
                {
                    var grid = new Grid { Padding = new Thickness(5, 10) };
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });


                    var icon = new Label();
                    icon.FontFamily = App.QFont;
                    icon.SetBinding(Label.TextProperty, "IconText");
                    icon.HorizontalTextAlignment = TextAlignment.Center;
                    icon.VerticalTextAlignment = TextAlignment.Center;
                    icon.FontSize = App.HeightUnit * 3;

                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "listName");

                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            label.FontSize = App.HeightUnit * 3;
                            break;
                        case Device.Android:
                            label.FontSize = App.HeightUnit * 2.2;
                            break;
                    }
                    var selectedLabel = new Label {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        FontFamily = App.QFont,
                        Text = QIcon.LEFT_DIR,
                        TextColor = Color.Green,
                        FontSize = App.HeightUnit * 3
                    };

                    grid.Children.Add(icon, 0, 0);
                    grid.Children.Add(label, 1, 0);
                    //grid.Children.Add(selectedLabel, 2, 0);
                    return new ViewCell { View = grid };

                })
            };
            StackLayout layout = new StackLayout();
            closeButton = new Button
            {
                BackgroundColor = Color.Gray,
                Text = "Close",
                TextColor = Color.White
            };

            layout.Children.Add(closeButton);
            layout.Children.Add(cueListView);
            Content = layout;

        }
    }
}
