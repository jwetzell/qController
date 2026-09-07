using Microsoft.Maui.Graphics;
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
            TextColor = Colors.Black;
            FontSize = App.WidthUnit * 5;
            if (qCommand.osc.Contains("go"))
            {
                BackgroundColor = Colors.SeaGreen;
                FontSize = App.HeightUnit * 4;
                FontAttributes = FontAttributes.Bold;
            }
            else if (qCommand.osc.Contains("panic"))
            {
                BackgroundColor = Colors.IndianRed;
            }
            else
            {
                BackgroundColor = Color.FromArgb("D8D8D8");
            }
        }
    }
}
