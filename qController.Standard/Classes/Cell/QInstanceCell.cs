using System;
using Xamarin.Forms;

namespace qController
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

            mainG.Children.Add(nameLabel, 1, 0);
            mainG.Children.Add(addressLabel,1,1);

            mainG.Children.Add(connectLabel,2,0);
            Grid.SetRowSpan(connectLabel,2);
            mainG.Children.Add(deleteLabel, 0, 0);
            Grid.SetRowSpan(deleteLabel,2);

            Frame f = new Frame
            {
                Content = mainG,
                OutlineColor = Color.Black,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 30,
                Margin = new Thickness(10,10,10,10),
                CornerRadius=20
            };
            f.BackgroundColor = Color.FromHex("D8D8D8");
            View = f;

            void InitItems()
            {
                nameLabel.HorizontalTextAlignment = TextAlignment.Center;
                nameLabel.VerticalTextAlignment = TextAlignment.End;
                nameLabel.FontAttributes = FontAttributes.Bold;
                nameLabel.FontSize = 20;

                addressLabel.HorizontalTextAlignment = TextAlignment.Center;
                addressLabel.VerticalTextAlignment = TextAlignment.Start;

                connectLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
                connectLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
                connectLabel.Text = QIcon.WIFI;
                connectLabel.FontSize = 40;
                connectLabel.TextColor = Color.LimeGreen;

                deleteLabel.HorizontalOptions = LayoutOptions.CenterAndExpand;
                deleteLabel.VerticalOptions = LayoutOptions.CenterAndExpand;
                deleteLabel.Text = QIcon.TRASH_EMPTY;
                deleteLabel.FontSize = 40;
                deleteLabel.TextColor = Color.Red;

                connectLabel.FontFamily = App.QFont;
                deleteLabel.FontFamily = App.QFont;
            }


            void Delete(object sender, EventArgs e)
            {

                Console.WriteLine("Delete " + nameLabel.Text + "," + addressLabel.Text + " Pressed");
                QStorage.RemoveInstance(nameLabel.Text,addressLabel.Text);

            }

            void Connect(object sender, EventArgs e)
            {
                App.NavigationPage.Navigation.PushAsync(new ControlPage(nameLabel.Text,addressLabel.Text));

                Console.WriteLine("Connect To " + nameLabel.Text + " Pressed");

            }
        }
    }
}
