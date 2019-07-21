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
                    Icon = cueList.IconText,
                    Command = "cueList " + cueList.uniqueID
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
