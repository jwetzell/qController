using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Xamarin.Forms;

namespace qController
{
    public partial class MenuPage : ContentPage
    {

        public ListView listView;
        public ObservableCollection<MenuPageItem> items;
        int subLevel = 0;
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
        public void ChangeToControl()
        {
            items.Clear();
            items.Add(new MenuPageItem
            {
                Title = "Disconnect",
                Icon = QIcon.CANCEL,
                Command = "disconnect"
            });
        }
        public void ChangeToWorkspace(QWorkSpace workspace)
        {
            ChangeToControl();
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
                    AddSubCues(cue, 0);
                }
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

            if (cue.cues != null )
            {          
                items.Add(new MenuPageItem
                {
                    Title = cueTitle,
                    Icon = cueIcon,
                    Command = "/select_id/" + cue.uniqueID
                });

                //uncomment to load nested group cues
                foreach (var sub_cue in cue.cues)
                {
                    AddSubCues(sub_cue,level + 1);
                }
            }
            else
            {
                items.Add(new MenuPageItem
                {
                    Title = cueTitle,
                    Icon = cueIcon,
                    Command = "/select_id/" + cue.uniqueID
                });
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
