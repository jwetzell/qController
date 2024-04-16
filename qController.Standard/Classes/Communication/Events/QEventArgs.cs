using System;
using System.Collections.Generic;
using qController.QItems;

namespace qController.Communication
{
    public class CueEventArgs : EventArgs
    {
        public QCue Cue
        {
            get;
            set;
        }
    }

    public class WorkspaceEventArgs : EventArgs
    {
        public QWorkspace UpdatedWorkspace
        {
            get;
            set;
        }
    }

    public class PlaybackPositionArgs : EventArgs
    {
        public string PlaybackPosition
        {
            get;
            set;
        }
    }

    public class ConnectEventArgs : EventArgs
    {
        public string Status
        {
            get;
            set;
        }

        public string WorkspaceId
        {
            get;
            set;
        }
    }

    public class WorkspaceInfoArgs : EventArgs
    {
        public List<QWorkspaceInfo> WorkspaceInfo
        {
            get;
            set;
        }
    }

    public class ChildrenEventArgs : EventArgs
    {
        public string cue_id
        {
            get;
            set;
        }
        public List<QCue> children
        {
            get;
            set;
        }
    }
}
