using System.Collections.Generic;
using Xamarin.Forms;

namespace qController.Cell
{
    public class QLevelsCell : Frame
    {

        public List<double> levels;
        public string activeCue;
        public Slider mainSlider;
        public Slider leftSlider;
        public Slider rightSlider;
        public Label mainLabel;
        public Label rightLabel;
        public Label leftLabel;
        public QLevelsCell()
        {
            Padding = new Thickness(5);
            CornerRadius = 20;
            BorderColor = Color.Black;
            HasShadow = true;
            BackgroundColor = Color.FromHex("D8D8D8");
            IsVisible = false;
            //highlight for testing
            //BackgroundColor = Color.Red;


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


            leftSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };


            rightSlider = new Slider
            {
                Minimum = -60,
                Maximum = 12
            };


            mainLabel = new Label
            {
                Text = "MAIN:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            leftLabel = new Label
            {
                Text = "1:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            rightLabel = new Label
            {
                Text = "2:",
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };

            mainG.Children.Add(mainSlider, 1, 0);
            mainG.Children.Add(leftSlider, 1, 1);
            mainG.Children.Add(rightSlider, 1, 2);

            Grid.SetColumnSpan(mainSlider, 4);
            Grid.SetColumnSpan(leftSlider, 4);
            Grid.SetColumnSpan(rightSlider, 4);

            mainG.Children.Add(mainLabel, 0, 0);
            mainG.Children.Add(leftLabel, 0, 1);
            mainG.Children.Add(rightLabel, 0, 2);

            Content = mainG;
            Margin = new Thickness(10);
        }

        public void UpdateLevels(List<double> lin_levels)
        {
            levels = lin_levels;
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
        }
    }
}