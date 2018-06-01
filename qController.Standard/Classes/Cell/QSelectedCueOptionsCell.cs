using System;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace qController
{
    public class QSelectedCueOptionsCell : Frame
    {
        void AudioLevelsUpdated(object source, AudioLevelArgs args)
        {
            Device.BeginInvokeOnMainThread(() => {
                levels = args.levels;
                if (mainSlider != null)
                {
                   mainSlider.Value = (double)levels[0];
                }
                if (leftSlider != null)
                {
                   leftSlider.Value = (double)levels[1];
                }
                if (rightSlider != null)
                {
                    rightSlider.Value = (double)levels[2];
                }
            });
        }

        Label type;
        JToken levels;
        Slider mainSlider;
        Slider leftSlider;
        Slider rightSlider;
        public QSelectedCueOptionsCell(QController qController)
        {
            qController.qParser.AudioLevelsUpdated += AudioLevelsUpdated;
            Padding = new Thickness(5);
            CornerRadius = 20;
            BackgroundColor = Color.FromHex("D8D8D8");
            Grid mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };
            type = new Label { Text = "" };

            mainSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };
            mainSlider.ValueChanged += (sender, args) =>
            {
                Console.WriteLine(String.Format("The Main Slider value is {0}", args.NewValue));
                qController.qSender.sendArgs("/cue/selected/sliderLevel/0", (float)args.NewValue);
            };

            leftSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };
            leftSlider.ValueChanged += (sender, args) =>
            {
                Console.WriteLine(String.Format("The Left Slider value is {0}", (float)args.NewValue));
                qController.qSender.sendArgs("/cue/selected/sliderLevel/1", (float)args.NewValue);
            };

            rightSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };
            rightSlider.ValueChanged += (sender, args) =>
            {
                Console.WriteLine(String.Format("The Right Slider value is {0}", args.NewValue));
                qController.qSender.sendArgs("/cue/selected/sliderLevel/2", (float)args.NewValue);
            };
            mainG.Children.Add(mainSlider, 1, 0);
            mainG.Children.Add(leftSlider, 1, 1);
            mainG.Children.Add(rightSlider, 1, 2);

            Grid.SetColumnSpan(mainSlider, 4);
            Grid.SetColumnSpan(leftSlider, 4);
            Grid.SetColumnSpan(rightSlider, 4);

            Label mainLabel = new Label { 
                Text = "MAIN",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            Label leftLabel = new Label
            {
                Text = "LEFT",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            Label rightLabel = new Label
            {
                Text = "RIGHT",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            mainG.Children.Add(mainLabel, 0, 0);
            mainG.Children.Add(leftLabel, 0, 1);
            mainG.Children.Add(rightLabel, 0, 2);

            Content = mainG;
        }

        public void UpdateSelectedCue(QCue cue)
        {
            type.Text = cue.type;
        }
    }
}
