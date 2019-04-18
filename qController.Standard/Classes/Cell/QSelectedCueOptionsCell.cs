using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace qController
{
    public class QSelectedCueOptionsCell : Frame
    {
        void AudioLevelsUpdated(object source, AudioLevelArgs args)
        {
            Device.BeginInvokeOnMainThread(() => {
                activeCue = args.cue_id;
                levels = args.levels;
                if (mainSlider != null)
                {
                   mainSlider.Value = levels[0];
                }
                if (leftSlider != null)
                {
                   leftSlider.Value = levels[1];
                }
                if (rightSlider != null)
                {
                    rightSlider.Value = levels[2];
                }
            });
        }

        List<double> levels;
        string activeCue;
        Slider mainSlider;
        Slider leftSlider;
        Slider rightSlider;
        public QSelectedCueOptionsCell(QController qController)
        {
            qController.qClient.qParser.AudioLevelsUpdated += AudioLevelsUpdated;
            Padding = new Thickness(5);
            CornerRadius = 20;
            BackgroundColor = Color.FromHex("D8D8D8");
            Grid mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Star},
                    new RowDefinition{Height = GridLength.Star},
                    new RowDefinition{Height = GridLength.Star}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };

            mainSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };
            mainSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs("/cue_id/" + activeCue + "/sliderLevel/0", (float)args.NewValue);
            };

            leftSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };
            leftSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs("/cue_id/" + activeCue + "/sliderLevel/1", (float)args.NewValue);
            };

            rightSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };
            rightSlider.ValueChanged += (sender, args) =>
            {
                qController.qClient.sendArgs("/cue_id/" + activeCue + "/sliderLevel/2", (float)args.NewValue);
            };
            mainG.Children.Add(mainSlider, 1, 0);
            mainG.Children.Add(leftSlider, 1, 1);
            mainG.Children.Add(rightSlider, 1, 2);

            Grid.SetColumnSpan(mainSlider, 4);
            Grid.SetColumnSpan(leftSlider, 4);
            Grid.SetColumnSpan(rightSlider, 4);

            Label mainLabel = new Label { 
                Text = "MAIN:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            Label leftLabel = new Label
            {
                Text = "LEFT:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            Label rightLabel = new Label
            {
                Text = "RIGHT:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            mainG.Children.Add(mainLabel, 0, 0);
            mainG.Children.Add(leftLabel, 0, 1);
            mainG.Children.Add(rightLabel, 0, 2);

            Content = mainG;
            Margin = new Thickness(10);
        }
    }
}
