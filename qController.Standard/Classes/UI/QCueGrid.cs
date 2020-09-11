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

            //Group List "Frame" added early so other children or "on top"
            if (cue.cues.Count > 0)
            {
                var cueFrame = new Frame
                {
                    BackgroundColor = Color.Transparent,
                    BorderColor = Color.Black
                };
                Children.Add(cueFrame);
                Grid.SetRowSpan(cueFrame, cue.cues.Count + 1);
            }

            var cueLabel = new Label
            {
                BindingContext = qCueViewModel,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = new Thickness(10,0,0,0)
            };
            cueLabel.SetBinding(Label.TextProperty, "name", BindingMode.OneWay);

            //Section for selecting a cue by tapping the name Label
            var selectCueGesture = new TapGestureRecognizer();
            selectCueGesture.Tapped += (sender, e) =>
            {
                cue.workspace.firstCueList.playbackPositionID = cue.uid;
            };
            cueLabel.GestureRecognizers.Add(selectCueGesture);

            var cueBackground = new Frame
            {
                BindingContext = qCueViewModel,
                Opacity = 0.50,
                HasShadow = false,
                CornerRadius = 0,
                BorderColor = Color.Black
            };

            cueBackground.SetBinding(BackgroundColorProperty, "color", BindingMode.OneWay);

            var cueSelectedIndicator = new Frame
            {
                BindingContext = qCueViewModel,
                BackgroundColor = Color.Blue,
                HasShadow = false
            };

            cueSelectedIndicator.SetBinding(IsVisibleProperty, "IsSelected");

            Children.Add(cueSelectedIndicator, 0, 0);
            Children.Add(cueBackground, 0, 0);
            Children.Add(cueLabel, 0, 0);
        }
    }
}
