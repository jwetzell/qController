using Xamarin.Forms;
using qController.ViewModels;
using QControlKit;

namespace qController.UI
{
    public class QCueGrid : Grid
    {
        public QCueGrid(QCue cue)
        {
            QCueViewModel qCueViewModel = new QCueViewModel(cue, true);

            RowSpacing = 0;
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition{Width = GridLength.Star},
                new ColumnDefinition{Width = new GridLength(5,GridUnitType.Star)}
            };

            //Group List "Frame" added early so other children or "on top"
            if (cue.cues.Count > 0)
            {
                var cueFrame = new Frame
                {
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.Black,
                    Margin = qCueViewModel.nestPadding
                };
                Children.Add(cueFrame,1,0);
                Grid.SetRowSpan(cueFrame, cue.cues.Count + 1);
                Grid.SetColumnSpan(cueFrame, ColumnDefinitions.Count-1);
            }

            var cueLabel = new Label
            {
                BindingContext = qCueViewModel,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0),
                //BackgroundColor = Color.Blue,
            };
            cueLabel.SetBinding(Label.TextProperty, "name", BindingMode.OneWay);
            cueLabel.SetDynamicResource(Label.TextColorProperty, "PrimaryTextColor");
            cueLabel.SetBinding(Label.MarginProperty, "nestPadding", BindingMode.OneWay);

            var cueTypeLabel = new Label
            {
                FontFamily = App.QFont,
                BindingContext = qCueViewModel,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 0,
                Margin = new Thickness(10,0,0,0),
                //BackgroundColor = Color.Red
            };

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    cueTypeLabel.FontSize = App.HeightUnit * 4;
                    break;
                default:
                    cueTypeLabel.FontSize = App.HeightUnit * 3;
                    break;
            }

            cueTypeLabel.SetBinding(Label.TextProperty, "type", BindingMode.OneWay);
            cueTypeLabel.SetDynamicResource(Label.TextColorProperty, "IconTextColor");

            
            if (!cue.IsCueList)
            {
                //Section for selecting a cue by tapping the name Label
                var selectCueGesture = new TapGestureRecognizer();
                selectCueGesture.Tapped += (sender, e) =>
                {
                    //TODO: need to make this work regardless of the cue list a cue is in
                    cue.workspace.firstCueList.playbackPositionID = cue.uid;
                };
                cueLabel.GestureRecognizers.Add(selectCueGesture);
            }

            //Add collapse gesture to the cue type icon if it is a group cue
            if (cue.IsGroup)
            {
                var CollapseGroupCueGesture = new TapGestureRecognizer();
                CollapseGroupCueGesture.Tapped += (sender, e) =>
                {
                    if (cue.IsGroup)
                    {
                        qCueViewModel.IsCollapsed = !qCueViewModel.IsCollapsed;
                        for (var i = 1; i < RowDefinitions.Count; i++)
                        {
                            RowDefinitions[i].Height = qCueViewModel.IsCollapsed ? 0 : GridLength.Auto;
                            
                        }

                        if (Device.RuntimePlatform.Equals(Device.Android))
                        {
                            foreach (var child in Children)
                            {
                                if (Grid.GetRow(child) > 0)
                                {
                                    child.IsVisible = !qCueViewModel.IsCollapsed;
                                }
                            }
                        }
                    }
                };
                cueTypeLabel.GestureRecognizers.Add(CollapseGroupCueGesture);
            }

            var cueBackground = new Frame
            {
                BindingContext = qCueViewModel,
                Opacity = 0.50,
                HasShadow = false,
                CornerRadius = 0,
                BorderColor = Color.Black
            };

            cueBackground.SetBinding(BackgroundColorProperty, "color", BindingMode.OneWay);

            //TODO: find a solution so the selected cue indicator is under the frame
            var cueSelectedIndicator = new Frame
            {
                BindingContext = qCueViewModel,
                BackgroundColor = Color.Blue,
                HasShadow = false
            };

            cueSelectedIndicator.SetBinding(IsVisibleProperty, "IsSelected");

            Children.Add(cueSelectedIndicator, 0, 0);
            Grid.SetColumnSpan(cueSelectedIndicator, ColumnDefinitions.Count);
            Children.Add(cueBackground, 0, 0);
            Grid.SetColumnSpan(cueBackground, ColumnDefinitions.Count);

            Children.Add(cueTypeLabel, 0, 0);
            Children.Add(cueLabel, 1, 0);
        }
    }
}
