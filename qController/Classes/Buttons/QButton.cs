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
                // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
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
                BackgroundColor = Colors.IndianRed;
            }
            else
            {
                BackgroundColor = Color.FromArgb("D8D8D8");
            }
        }
    }
}
