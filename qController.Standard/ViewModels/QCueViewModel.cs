using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using QControlKit;
using QControlKit.Constants;
using QControlKit.Events;
using System.Collections.Generic;
using qController.Helpers;

namespace qController.ViewModels
{
    public class QCueViewModel : INotifyPropertyChanged
    {
        public QCue cue;
        bool isSelected = false;
        bool isCollapsed = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public QCueViewModel(QCue cue, bool checkPlayback)
        {
            this.cue = cue;
            if (checkPlayback) {
                this.cue.workspace.CueListChangedPlaybackPosition += Workspace_CueListChangedPlaybackPosition;
            }
            this.cue.CuePropertiesUpdated += OnCuePropertiesUpdated;
        }

        private void OnCuePropertiesUpdated(object source, QCuePropertiesUpdatedArgs args)
        {
            foreach (string property in args.properties)
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
                    if (cue.IsGroup)
                    {
                        OnPropertyChanged("cues");
                    }
                }
                else if (property.Equals(QOSCKey.IsBroken))
                {
                    OnPropertyChanged("status");
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

        public Thickness nestPadding => new Thickness((cue.nestLevel - 1) * 10, 0, 0, 0);

        public string name
        {
            get => cue.listName;

            set => cue.name = value;
        }

        public string type => QIcon.GetIconFromType(cue.type);

        public string uid => cue.uid;

        public string number
        {
            get => cue.number;

            set => cue.number = value;
        }

        public string notes
        {
            get => cue.notes;

            set => cue.notes = value;
        }

        public Color color
        {
            get => Color.FromHex(cue.color.Hex);
            set => cue.color = new QColor("none");
        }

        public List<QCue> cues => cue.cues;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public bool IsCollapsed
        {
            get => isCollapsed;
            set
            {
                isCollapsed = value;
                OnPropertyChanged();
            }
        }

        public string status
        {
            get
            {
                if (cue.IsRunning)
                {
                    return IconConstants.Play;
                }
                else if (cue.IsBroken)
                {
                    return IconConstants.Cancel;
                }
                else
                {
                    return " ";
                }
            }
        }
    }
}
