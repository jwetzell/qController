using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace qController
{
    public partial class MenuPage : ContentPage
    {

        public ListView listView;
        public ObservableCollection<MenuPageItem> items;
        public MenuPage()
        {
            Title = "Menu";
            InitializeComponent();
            items = new ObservableCollection<MenuPageItem>();

            items.Add(new MenuPageItem
            {
                Title = "Scan Network",
                Icon = "\uE800",
                Command = "scan"
            });
            items.Add(new MenuPageItem
            {
                Title = "Add Manually",
                Icon = "\uE801",
                Command = "add"
            });
            /*items.Add(new MenuPageItem
            {
                Title = "Open Link",
                Icon = "\uF0C9",
                Command = "open_link"
            });*/
            listView = new ListView
            {
                ItemsSource = items,
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
                    icon.FontSize = 20;
                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Title");
                    if (label.Text == "Disconnect")
                    {
                        label.TextColor = Color.DarkRed;
                    }
                    grid.Children.Add(icon);
                    grid.Children.Add(label, 1, 0);
                    return new ViewCell { View = grid };
                })
            };
            Content = new StackLayout
            {
                Children = { listView }
            };

        }

        public void ChangeToWorkspace(QWorkSpace workspace)
        {
            items.Clear();
            items.Add(new MenuPageItem
            {
                Title = "Disconnect",
                Icon = "\uE803",
                Command = "disconnect"
            });
            foreach (var cueList in workspace.data)
            {
                items.Add(new MenuPageItem
                {
                    Title = cueList.listName,
                    Icon = "",
                    Command = ""
                });
                foreach (var cue in cueList.cues)
                {
                    var cueIcon = cue.getIconString();
                    var cueTitle = "";

                    if(cue.number != "")
                    {
                        cueTitle = "\t" + cue.number + " - " + cue.listName;
                    }
                    else
                    {
                        cueTitle = "\t" + cue.listName;
                    }

                    items.Add(new MenuPageItem
                    {
                        Title = cueTitle,
                        Icon = cueIcon,
                        Command = "/select_id/" + cue.uniqueID
                    });

                }
            }
        }

        public void ChangeToHome()
        {
            items.Clear();
            items.Add(new MenuPageItem
            {
                Title = "Scan Network",
                Icon = "\uE800",
                Command = "scan"
            });
            items.Add(new MenuPageItem
            {
                Title = "Add Manually",
                Icon = "\uE801",
                Command = "add"
            });
        }
    }
}
