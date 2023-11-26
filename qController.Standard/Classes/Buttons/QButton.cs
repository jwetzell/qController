using Xamarin.Forms;

namespace qController
{
    public class QButton : Button
    {
        public QCommand qCommand
        {
            get;
            set;
        }

        public QButton(QCommand command)
        {
            
            qCommand = command;
            Text = qCommand.text;
            TextColor = Color.Black;
            FontSize = App.WidthUnit * 5;
            if (qCommand.osc.Contains("go"))
            {
                BackgroundColor = Color.SeaGreen;
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        FontSize = App.HeightUnit * 4;
                        FontAttributes = FontAttributes.Bold;
                        break;
                    case Device.Android:
                        FontSize = App.HeightUnit * 4;
                        FontAttributes = FontAttributes.Bold;
                        break;
                }
            }
            else if (qCommand.osc.Contains("panic"))
            {
                BackgroundColor = Color.IndianRed;
            }
            else
            {
                BackgroundColor = Color.FromHex("D8D8D8");
            }
        }
    }
}
