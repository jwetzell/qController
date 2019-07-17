using System;
using System.Collections.ObjectModel;
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

            ChangeToHome();
            
            listView = new ListView
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
                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Title");
                    if (label.Text == "Disconnect")
                    {
                        label.TextColor = Color.DarkRed;
                    }
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
                    return new ViewCell {View = grid};
                  
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
        public void ChangeToWorkspace(QWorkspace workspace)
        {
            for(int i = 0; i < workspace.data.Count; i++)
            {
                var cueList = workspace.data[i];
                items.Add(new MenuPageItem
                {
                    Title = cueList.listName,
                    Icon = "",
                    Command = ""
                });
                for (int j = 0; j < cueList.cues.Count; j++)
                {
                    var cue = cueList.cues[j];
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
                for (int i = 0; i < cue.cues.Count; i++)
                {
                    var sub_cue = cue.cues[i];
                    AddSubCues(sub_cue, level + 1);
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

        public void ChangeCueName(string cue_id, string name){
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].Command.Contains(cue_id))
                {
                    var item = items[i];
                    items.RemoveAt(i);
                    item.Title = name;
                    items.Insert(i, item);
                    Console.WriteLine("CUE OBJECT FOUND IN MENU TRYING TO UPDATE");
                    return;
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
            items.Add(new MenuPageItem
            {
                Title = "Send Feedback",
                Icon = QIcon.MAIL,
                Command = "feedback"
            });
            items.Add(new MenuPageItem
            {
                Title = "Support Project",
                Icon = QIcon.DOLLAR,
                Command = "support"
            });
        }
    }
}
