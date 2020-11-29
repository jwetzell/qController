using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Constants;
using QControlKit.Events;

namespace qController.ViewModels
{
    public class QCueViewModel : INotifyPropertyChanged
    {
        QCue cue;
        bool isSelected = false;
        bool isCollapsed = false;
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
                if (property.Equals(QOSCKey.Name) || property.Equals(QOSCKey.ListName))
                {
                    OnPropertyChanged("name");
                }
                else if (property.Equals(QOSCKey.Number))
                {
                    OnPropertyChanged("number");
                }
                else if (property.Equals(QOSCKey.Notes))
                {
                    OnPropertyChanged("notes");
                }
                else if (property.Equals(QOSCKey.ColorName))
                {
                    OnPropertyChanged("color");
                }
                else if (property.Equals(QOSCKey.Children))
                {
                    Log.Debug("children property updated");
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

        public Thickness nestPadding
        {
            get
            {
                return new Thickness((cue.nestLevel - 1) * 10,0,0,0);
            }
        }

        public string name
        {
            get
            {
                return cue.listName;
            }

            set
            {
                cue.name = value;
            }
        }

        public string type
        {
            get
            {
                return QIcon.GetIconFromType(cue.type);
            }
        }

        public string uid
        {
            get
            {
                return cue.uid;
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

        public string notes
        {
            get
            {
                return cue.notes;
            }

            set
            {
                cue.notes = value;
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

        public bool IsCollapsed
        {
            get
            {
                return isCollapsed;
            }
            set
            {
                isCollapsed = value;
                OnPropertyChanged();
            }
        }
    }
}
