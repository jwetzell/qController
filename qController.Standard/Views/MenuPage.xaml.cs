﻿using System.Collections.ObjectModel;
using Xamarin.Forms;
using qController.QItems;
using System;

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

                //switch (Device.RuntimePlatform)
                //{
                //    case Device.iOS:
                //        listView.RowHeight = (int)(App.HeightUnit * 5);
                //        label.FontSize = App.HeightUnit * 2.75;
                //        break;
                //    case Device.Android:
                //        listView.RowHeight = (int)(App.HeightUnit * 5);
                //        label.FontSize = App.HeightUnit * 2.75;
                //        break;
                //}

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
                Icon = QIcon.CANCEL,
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
                Icon = "\uE800",
                Command = "scan"
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
