﻿using Acr.UserDialogs;
using qController.ViewModels;
using Xamarin.Forms;

namespace qController.UI
{
    public class QSelectedCueFrame : Frame
    {
        public QSelectedCueFrame()
        {
            //Colored border size
            Padding = 8;
            HeightRequest = 175;
            CornerRadius = 0;
            this.SetBinding(Frame.BackgroundColorProperty, "color", BindingMode.OneWay);
            this.SetBinding(Frame.BorderColorProperty, "color", BindingMode.OneWay);

            Frame frameInt = new Frame
            {
                Padding = 0
            };

            frameInt.SetDynamicResource(Frame.BackgroundColorProperty, "SelectedCueCellBackgroundColor");


            Grid selectedCueGrid = new Grid
            {
                Margin = 0,
                Padding = 0,
                RowSpacing = 0,
                ColumnSpacing = 0,
                BackgroundColor = Color.Transparent,
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = new GridLength(2, GridUnitType.Star) }
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };


            Label number = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = 0,
                Padding = new Thickness(15,5,0,0),
                FontSize = App.HeightUnit * 3
            };
            number.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");
            number.SetBinding(Label.TextProperty, "number", BindingMode.OneWay);
            selectedCueGrid.Children.Add(number,0,0);
            Grid.SetColumnSpan(number, 2);

            Label type = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = 0,
                Padding = new Thickness(0,0,15,0),
                FontSize = App.HeightUnit * 4,
                FontFamily = App.QFont
            };
            type.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");
            type.SetBinding(Label.TextProperty, "type", BindingMode.OneWay);
            selectedCueGrid.Children.Add(type, 4, 0);


            Label name = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Transparent,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            name.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");
            name.SetBinding(Label.TextProperty, "name", BindingMode.TwoWay);
            selectedCueGrid.Children.Add(name, 0, 1);
            Grid.SetColumnSpan(name, 5);

            Editor notes = new Editor
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = 5,
                BackgroundColor = Color.Transparent,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Editor))
            };
            notes.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");
            notes.SetBinding(Editor.TextProperty, "notes", BindingMode.TwoWay);

            selectedCueGrid.Children.Add(notes, 0, 2);
            Grid.SetColumnSpan(notes, 5);

            //Double tap to edit name/number

            var nameDoubleTap = new TapGestureRecognizer();
            nameDoubleTap.NumberOfTapsRequired = 2;

            nameDoubleTap.Tapped += (s, e) =>
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    Title = "Update Name",
                    Message = "Change name to update",
                    OkText = "Update",
                    Text = name.Text,
                    OnAction = (qName) =>
                    {
                        if (!qName.Ok)
                            return;
                        name.Text = qName.Text;
                    }
                });
            };

            name.GestureRecognizers.Add(nameDoubleTap);

            var numberDoubleTap = new TapGestureRecognizer();
            numberDoubleTap.NumberOfTapsRequired = 2;

            numberDoubleTap.Tapped += (s, e) =>
            {
                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    Title = "Update Number",
                    Message = "Change number to update",
                    OkText = "Update",
                    Text = number.Text,
                    OnAction = (qNumber) =>
                    {
                        if (!qNumber.Ok)
                            return;
                        //This bypasses the label .Text property because the number might not be valid (no duplicates)
                        ((QCueViewModel)BindingContext).number = qNumber.Text;
                    }
                });
            };

            number.GestureRecognizers.Add(numberDoubleTap);


            //BACKGROUND COLORS FOR TESTING ONLY 
            //notes.BackgroundColor = Color.Red;
            //number.BackgroundColor = Color.Blue;
            //name.BackgroundColor = Color.Green;
            //type.BackgroundColor = Color.Yellow;

            //setup contents
            frameInt.Content = selectedCueGrid;
            Content = frameInt;
        }
    }
}