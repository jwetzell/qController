using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace qController
{
    public partial class MenuPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        ListView listView;

        public MenuPage()
        {
            Title = "Menu";
            InitializeComponent();
            var items = new List<MasterPageItem>();
            items.Add(new MasterPageItem
            {
                Title = "Scan Network",
                Icon = "\uE800",
                Command = "scan"
            });
            items.Add(new MasterPageItem
            {
                Title = "Add Manually",
                Icon = "\uE801",
                Command = "add"
            });
            /*items.Add(new MasterPageItem
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

                    string qfont = "";

                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            qfont = "qfont";
                            break;
                        case Device.Android:
                            qfont = "qfont.ttf#qfont";
                            break;

                    }
                    var icon = new Label();

                    icon.FontFamily = qfont as string;
                    icon.SetBinding(Label.TextProperty, "Icon");
                    icon.HorizontalTextAlignment = TextAlignment.Center;
                    icon.VerticalTextAlignment = TextAlignment.Center;
                    icon.FontSize = 20;
                    var label = new Label { VerticalOptions = LayoutOptions.FillAndExpand };
                    label.SetBinding(Label.TextProperty, "Title");

                    grid.Children.Add(icon);
                    grid.Children.Add(label, 1, 0);
                    return new ViewCell { View = grid };
                }),
                SeparatorVisibility = SeparatorVisibility.None
            };
            Content = new StackLayout
            {
                Children = { listView }
            };

        }
    }
}
