﻿using Serilog;

namespace qController.Cell
{
    public class QInstanceCell : ViewCell
    {
        public QInstanceCell()
        {
            

            Label nameLabel = new Label();
            Label addressLabel = new Label();

            Label connectLabel = new Label();
            Label deleteLabel = new Label();

            var connectTapGesture = new TapGestureRecognizer();
            var deleteTapGesture = new TapGestureRecognizer();

            connectTapGesture.Tapped += Connect;
            connectLabel.GestureRecognizers.Add(connectTapGesture);


            deleteTapGesture.Tapped += Delete;
            deleteLabel.GestureRecognizers.Add(deleteTapGesture);

            InitItems();
          

            //SET BINDINGS
            nameLabel.SetBinding(Label.TextProperty, new Binding("name"));
            addressLabel.SetBinding(Label.TextProperty,new Binding("address"));

            Grid mainG = new Grid
            {
               //Padding = new Thickness(10),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(4,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };

            mainG.Add(nameLabel, 1, 0);
            mainG.Add(addressLabel,1,1);

            mainG.Add(connectLabel,2,0);
            Grid.SetRowSpan(connectLabel,2);
            mainG.Add(deleteLabel, 0, 0);
            Grid.SetRowSpan(deleteLabel,2);

            Frame f = new Frame
            {
                Content = mainG,
                BorderColor = Colors.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Margin = new Thickness(10,10,10,10),
                CornerRadius=20,
                BackgroundColor = Color.FromArgb("D8D8D8")
            };
            View = f;
            
           
            void InitItems()
            {
                
                nameLabel.HorizontalTextAlignment = TextAlignment.Center;
                nameLabel.VerticalTextAlignment = TextAlignment.End;
                nameLabel.FontAttributes = FontAttributes.Bold;
                nameLabel.FontSize = App.HeightUnit * 3;
                nameLabel.Margin = new Thickness(0, 20, 0, 0);

                addressLabel.HorizontalTextAlignment = TextAlignment.Center;
                addressLabel.VerticalTextAlignment = TextAlignment.Start;
                addressLabel.FontSize = App.HeightUnit * 2.5;
                addressLabel.Margin = new Thickness(0, 0, 0, 20);

                connectLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
                connectLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
                connectLabel.Text = QIcon.WIFI;
                connectLabel.FontSize = App.HeightUnit * 5;
                connectLabel.TextColor = Colors.LimeGreen;

                deleteLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
                deleteLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
                deleteLabel.Text = QIcon.TRASH_EMPTY;
                deleteLabel.FontSize = App.HeightUnit * 5;
                deleteLabel.TextColor = Colors.Red;

                connectLabel.FontFamily = App.QFont;
                deleteLabel.FontFamily = App.QFont;
            }


            void Delete(object sender, EventArgs e)
            {

                Log.Debug("QINSTANCECELL - Delete " + nameLabel.Text + "," + addressLabel.Text + " Pressed");
                QStorage.RemoveInstance(nameLabel.Text,addressLabel.Text);

            }

            void Connect(object sender, EventArgs e)
            {
                App.rootPage.Detail.Navigation.PushAsync(new ControlPage(nameLabel.Text, addressLabel.Text));
                Log.Debug("QINSTANCECELL - Connect To " + nameLabel.Text + " Pressed");
            }
        }
    }
}
