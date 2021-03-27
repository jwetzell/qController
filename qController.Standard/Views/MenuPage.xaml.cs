using System.Collections.ObjectModel;
using Xamarin.Forms;
using qController.QItems;
using System;
using qController.Helpers;

namespace qController
{
    public partial class MenuPage : ContentPage
    {

        public ObservableCollection<MenuPageItem> items { get; } = new ObservableCollection<MenuPageItem>();
        public MenuPage()
        {
            Title = "Menu";
            InitializeComponent();

            ChangeToHome();

            listView.ItemsSource = items;
            listView.ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid {
                    Padding = 0,
                    ColumnSpacing = 0,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{Width = GridLength.Auto},
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };

                var icon = new Label {
                    FontFamily = (OnPlatform<string>)Application.Current.Resources["MaterialFontFamily"],
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(10,0,10,0),
                    FontSize = App.HeightUnit * 2.75
                };

                icon.SetBinding(Label.TextProperty, "Icon");
                icon.SetDynamicResource(Label.TextColorProperty, "IconTextColor");

                var label = new Label {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = App.HeightUnit * 2.75
                
                };
                label.SetBinding(Label.TextProperty, "Title");
                label.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");

                grid.Children.Add(icon);
                grid.Children.Add(label, 1, 0);
                return new ViewCell { View = grid };
            });

        }

        public void setItemSelected(EventHandler<SelectedItemChangedEventArgs> ItemSelected){
            listView.ItemSelected += ItemSelected;
        }

        public void clearSelectedItem()
        {
            listView.SelectedItem = null;
        }

        public void ChangeToControl()
        {
            items.Clear();
            items.Add(new MenuPageItem
            {
                Title = "Disconnect",
                Icon = IconConstants.Close,
                Command = "disconnect"
            });
            
        }

        public void ChangeToWorkspace(QOldWorkspace workspace)
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
                Icon = IconConstants.Magnify,
                Command = "scan"
            });
            items.Add(new MenuPageItem
            {
                Title = "Send Feedback",
                Icon = IconConstants.Email,
                Command = "feedback"
            });
            items.Add(new MenuPageItem
            {
                Title = "Support Project",
                Icon = IconConstants.CurrencyUsd,
                Command = "support"
            });
        }
    }
}
