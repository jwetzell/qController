using QSharp;
using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace qController.ViewModels
{
    public class QCueViewModel : INotifyPropertyChanged
    {
        QCue cue;
        bool isSelected = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public QCueViewModel(QCue cue, bool checkPlayback)
        {
            this.cue = cue;
            if (checkPlayback)
                this.cue.workspace.CueListChangedPlaybackPosition += Workspace_CueListChangedPlaybackPosition;
            this.cue.CuePropertiesUpdated += OnCuePropertiesUpdated;
        }

        private void OnCuePropertiesUpdated(object source, QCuePropertiesUpdatedArgs args)
        {
            foreach (var property in args.properties)
            {
                Log.Debug($"[cueviewmodel] property <{property}> has been updated.");
                if (property.Equals(QOSCKey.Name))
                {
                    OnPropertyChanged("name");
                }
                else if (property.Equals(QOSCKey.ColorName))
                {
                    OnPropertyChanged("color");
                }
            }
        }

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Workspace_CueListChangedPlaybackPosition(object source, QCueListChangedPlaybackPositionArgs args)
        {
            IsSelected = args.cueID == cue.uid;
        }

        public string name
        {
            get
            {
                return cue.displayName;
            }

            set
            {
                cue.name = value;
            }
        }

        public string number
        {
            get
            {
                return cue.number;
            }

            set
            {
                cue.number = value;
            }
        }

        public Color color
        {
            get
            {
                return Color.FromHex(cue.color.Hex);
            }
            set
            {
                cue.color = new QColor("none");
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
