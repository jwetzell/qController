using System;
using System.Collections.Generic;
using System.Text;

namespace qController.Communication
{
    public delegate void SelectedCueUpdatedHandler(object source, CueEventArgs args);
    public delegate void WorkspaceUpdatedHandler(object source, WorkspaceEventArgs args);
    public delegate void CueInfoUpdatedHandler(object source, CueEventArgs args);
    public delegate void PlaybackPositionUpdatedHandler(object source, PlaybackPositionArgs args);
    public delegate void ConnectionStatusHandler(object source, ConnectEventArgs args);
    public delegate void ChildrenUpdateHandler(object source, ChildrenEventArgs args);
    public delegate void WorkspaceDisconnectHandler(object source, EventArgs args);
    public delegate void WorkspaceInfoHandler(object source, WorkspaceInfoArgs args);
    public delegate void WorkspaceLoadErrorHandler(object source, WorkspaceEventArgs args);
}
