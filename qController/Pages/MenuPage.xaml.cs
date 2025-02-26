﻿using System.Collections.ObjectModel;
using qController.QItems;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace qController
{
    public partial class MenuPage : ContentPage
    {

        public ObservableCollection<MenuPageItem> items { get; } = new ObservableCollection<MenuPageItem>();
        public MenuPage()
        {
            On<iOS>().SetUseSafeArea(true);
            Title = "Menu";
            InitializeComponent();

            ChangeToHome();

            listView.ItemsSource = items;
            listView.RowHeight = (int)(App.HeightUnit * 6);
            listView.ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    Padding = new Thickness(5, 10),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition{Width = new GridLength(30)},
                        new ColumnDefinition { Width = GridLength.Star }
                    }
                };


                var icon = new Label {
                    FontFamily = App.QFont,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = App.HeightUnit * 3
                };

                icon.SetBinding(Label.TextProperty, "Icon");

                var label = new Label {
                    VerticalOptions = LayoutOptions.FillAndExpand
                };
                label.SetBinding(Label.TextProperty, "Title");

                if (label.Text == "Disconnect")
                    label.TextColor = Colors.DarkRed;

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


                grid.Add(icon);
                grid.Add(label, 1, 0);
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
