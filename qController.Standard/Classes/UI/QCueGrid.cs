using Xamarin.Forms;
using qController.ViewModels;
using QControlKit;
using System.Linq;
using System.Collections.Generic;

namespace qController.UI
{
    public class QCueGrid : Grid
    {
        private QCueViewModel qCueViewModel;

        public QCueGrid(QCue cue)
        {
            qCueViewModel = new QCueViewModel(cue, true);
            qCueViewModel.PropertyChanged += QCueViewModel_PropertyChanged;
            RowSpacing = 0;

            BuildGrid(cue);
            
        }

        private void BuildGrid(QCue cue)
        {
            RowDefinitions.Add(
                new RowDefinition
                {
                    Height = new GridLength(40),
                    BindingContext = qCueViewModel
                }
            );
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition{Width = GridLength.Star},
                new ColumnDefinition{Width = new GridLength(5,GridUnitType.Star)}
            };

            //Group List "Frame" added early so other children or "on top"
            if (cue.cues.Count > 0)
            {
                Frame cueFrame = new Frame
                {
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.Black,
                    Margin = qCueViewModel.nestPadding
                };
                Children.Add(cueFrame, 1, 0);
                SetRowSpan(cueFrame, cue.cues.Count + 1);
                SetColumnSpan(cueFrame, ColumnDefinitions.Count - 1);
            }

            Label cueLabel = new Label
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

            Label cueTypeLabel = new Label
            {
                FontFamily = (OnPlatform<string>)Application.Current.Resources["QFontFamily"],
                BindingContext = qCueViewModel,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 0,
                Margin = new Thickness(10, 0, 0, 0),
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
                TapGestureRecognizer selectCueGesture = new TapGestureRecognizer();
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
                TapGestureRecognizer CollapseGroupCueGesture = new TapGestureRecognizer();
                CollapseGroupCueGesture.Tapped += (sender, e) =>
                {
                    if (cue.IsGroup)
                    {
                        qCueViewModel.IsCollapsed = !qCueViewModel.IsCollapsed;
                        for (int i = 1; i < RowDefinitions.Count; i++)
                        {
                            RowDefinitions[i].Height = qCueViewModel.IsCollapsed ? 0 : GridLength.Auto;

                        }

                        if (Device.RuntimePlatform.Equals(Device.Android))
                        {
                            foreach (View child in Children)
                            {
                                if (GetRow(child) > 0)
                                {
                                    child.IsVisible = !qCueViewModel.IsCollapsed;
                                }
                            }
                        }
                    }
                };
                cueTypeLabel.GestureRecognizers.Add(CollapseGroupCueGesture);
            }

            Frame cueBackground = new Frame
            {
                BindingContext = qCueViewModel,
                Opacity = 0.50,
                HasShadow = false,
                CornerRadius = 0,
                BorderColor = Color.Black
            };

            cueBackground.SetBinding(BackgroundColorProperty, "color", BindingMode.OneWay);

            //TODO: find a solution so the selected cue indicator is under the frame
            Frame cueSelectedIndicator = new Frame
            {
                BindingContext = qCueViewModel,
                BackgroundColor = Color.Blue,
                HasShadow = false
            };

            cueSelectedIndicator.SetBinding(IsVisibleProperty, "IsSelected");

            Children.Add(cueSelectedIndicator, 0, 0);
            SetColumnSpan(cueSelectedIndicator, ColumnDefinitions.Count);
            Children.Add(cueBackground, 0, 0);
            SetColumnSpan(cueBackground, ColumnDefinitions.Count);

            Children.Add(cueTypeLabel, 0, 0);
            Children.Add(cueLabel, 1, 0);

            if (cue.cues.Count > 0)
            {
                foreach (QCue aCue in cue.cues)
                {
                    RowDefinitions.Add(
                        new RowDefinition
                        {
                            Height = GridLength.Auto,
                            BindingContext = new QCueViewModel(aCue, false)
                        }
                    );
                    QCueGrid aCueGrid = new QCueGrid(aCue)
                    {
                        //cueGridDict.Add(aCue.uid, aCueGrid);
                        Margin = new Thickness(0, 0, 0, 0)
                    };
                    QCueGridListHelper.insert(aCue.uid, aCueGrid);
                    Children.Add(aCueGrid, 0, aCue.sortIndex + 1);
                    SetColumnSpan(aCueGrid, ColumnDefinitions.Count);
                }
            }
        }

        private void ReloadGrid()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //Nuke current grid
                List<View> children = Children.ToList();
                foreach (View child in children)
                {
                    Children.Remove(child);
                }

                RowDefinitions = new RowDefinitionCollection();

                //rebuild
                BuildGrid(qCueViewModel.cue);

                //TODO: check if this is working
                qCueViewModel.cue.workspace.fetchPlaybackPositionForCue(qCueViewModel.cue);
            });
        }

        private void QCueViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("cues"))
            {
                ReloadGrid();
            }
        }
    }
}
