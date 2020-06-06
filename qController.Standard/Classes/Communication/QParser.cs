﻿using System;
using SharpOSC;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Serilog;

namespace qController
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

    public class QParser
    {
        public QParser()
        {
           
        }

        public delegate void SelectedCueUpdatedHandler(object source, CueEventArgs args);
        public delegate void WorkspaceUpdatedHandler(object source, WorkspaceEventArgs args);
        public delegate void CueInfoUpdatedHandler(object source, CueEventArgs args);
        public delegate void PlaybackPositionUpdatedHandler(object source, PlaybackPositionArgs args);
        public delegate void ConnectionStatusHandler(object source, ConnectEventArgs args);
        public delegate void ChildrenUpdateHandler(object source, ChildrenEventArgs args);
        public delegate void WorkspaceDisconnectHandler(object source, EventArgs args);
        public delegate void WorkspaceInfoHandler(object source, WorkspaceInfoArgs args);
        public delegate void WorkspaceLoadErrorHandler(object source, WorkspaceEventArgs args);
        public event WorkspaceLoadErrorHandler WorkspaceLoadError;
        public event WorkspaceInfoHandler WorkspaceInfoReceived;
        public event WorkspaceDisconnectHandler WorkspaceDisconnect;
        public event ChildrenUpdateHandler ChildrenUpdated;
        public event ConnectionStatusHandler ConnectionStatusChanged;
        public event SelectedCueUpdatedHandler SelectedCueUpdated;
        public event WorkspaceUpdatedHandler WorkspaceUpdated;
        public event CueInfoUpdatedHandler CueInfoUpdated;
        public event PlaybackPositionUpdatedHandler PlaybackPositionUpdated;

        public void ParseMessage(OscMessage msg){
            if (!msg.Address.Contains("null"))
            {
                if (msg.Address.Contains("valuesForKeys"))
                {
                    ParseCueUpdateInfo(msg);
                }
                else if (msg.Address.Contains("cueLists"))
                    ParseWorkspaceInfo(msg);
                else if (msg.Address.Contains("playbackPosition"))
                    ParsePositionUpdateInfo(msg);
                else if (msg.Address.Contains("thump"))
                    Log.Debug("QPARSER - Heartbeat Received");
                else if (msg.Address.Contains("disconnect"))
                    OnWorkspaceDisconnect();
                else if (msg.Address.Contains("connect"))
                    ParseConnectInfo(msg);
                else if (msg.Address.Contains("children"))
                    ParseChildrenInfo(msg);
                else if (msg.Address.Contains("workspaces"))
                    ParseQInfo(msg);
                else
                {
                    Log.Debug("QPARSER - Unkown message type: " + msg.Address);
                    foreach (var item in msg.Arguments)
                    {
                        Log.Debug(item.ToString());
                    }
                }
            }
        }
        
        public void ParseConnectInfo(OscMessage msg)
        {
            if (msg.Arguments.Count > 0)
            {
                JToken connectStatus = OSC2JSON(msg);
                ;
                OnConnectionStatusChanged(connectStatus.ToString(), msg.Address.Split('/')[3]);
            }
        }

        public void ParsePositionUpdateInfo(OscMessage msg)
        {
            if(msg.Arguments.Count > 0)
            {
                OnPlaybackPositionUpdated(msg.Arguments[0].ToString());
            }
        }

        public void ParseCueUpdateInfo(OscMessage msg)
        {
            JToken cueUpdate = OSC2JSON(msg);
            QCue cue = JsonConvert.DeserializeObject<QCue>(cueUpdate.ToString());
            OnCueInfoUpdated(cue);
        }

        public void ParseSelectedCueInfo(OscMessage msg){
            JToken selectedCue = OSC2JSON(msg);
            QCue cue = JsonConvert.DeserializeObject<QCue>(selectedCue.ToString());
            OnSelectedCueUpdated(cue);
        }

        public void ParseQInfo(OscMessage msg)
        {
            JToken qInfo = OSC2JSON(msg);
            List<QWorkspaceInfo> qWorkspaceInfo = JsonConvert.DeserializeObject<List<QWorkspaceInfo>>(qInfo.ToString());
            OnWorkspaceInfoReceived(qWorkspaceInfo);
        }

        public void ParseWorkspaceInfo(OscMessage msg)
        {
            if (msg.Arguments.Count > 0)
            {
                var parts = msg.Address.Split('/');
                string id = parts[3];

                if (msg.Arguments[0].ToString() != "")
                {
                    try
                    {
                        QWorkspace workspace = JsonConvert.DeserializeObject<QWorkspace>(msg.Arguments[0].ToString());
                        OnWorkspaceUpdated(workspace);
                    }
                    catch (Exception ex)
                    {
                        OnWorkspaceLoadError(id);
                    }
                }
                else
                {
                    OnWorkspaceLoadError(id);
                }
            }
        }

        public void ParseChildrenInfo(OscMessage msg)
        {
            if (msg.Arguments.Count > 0)
            {
                string cue_id = msg.Address.Split('/')[3];
                JToken children_obj = OSC2JSON(msg);
                List<QCue> children = JsonConvert.DeserializeObject<List<QCue>>(children_obj.ToString());
                OnChildrenUpdated(cue_id, children);
            }
        }

        public JToken OSC2JSON(OscMessage Msg){
            JObject json = JObject.Parse(Msg.Arguments.ToArray()[0].ToString());
            return json.GetValue("data");
        }

        protected virtual void OnWorkspaceUpdated(QWorkspace workspace)
        {
            if (WorkspaceUpdated != null)
                WorkspaceUpdated(this, new WorkspaceEventArgs() { UpdatedWorkspace = workspace });
        }
        protected virtual void OnSelectedCueUpdated(QCue cue)
        {
            if (SelectedCueUpdated != null)
                SelectedCueUpdated(this, new CueEventArgs() { Cue = cue });
        }

        protected virtual void OnCueInfoUpdated(QCue cue)
        { 
            if (CueInfoUpdated != null)
                CueInfoUpdated(this, new CueEventArgs() { Cue = cue });
        }

        protected virtual void OnPlaybackPositionUpdated(string id)
        {
            if (PlaybackPositionUpdated != null)
                PlaybackPositionUpdated(this, new PlaybackPositionArgs() { PlaybackPosition = id });
        }
        protected virtual void OnConnectionStatusChanged(string status, string workspace_id)
        {
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectEventArgs() { Status = status, WorkspaceId = workspace_id });
        }
        protected virtual void OnChildrenUpdated(string id, List<QCue> cues)
        {
            if (ChildrenUpdated != null)
                ChildrenUpdated(this, new ChildrenEventArgs() { cue_id = id, children = cues });
        }
        protected virtual void OnWorkspaceDisconnect()
        {
            if (WorkspaceDisconnect != null)
                WorkspaceDisconnect(this, new EventArgs());
        }
        protected virtual void OnWorkspaceInfoReceived(List<QWorkspaceInfo> workspaces)
        {
            if (WorkspaceInfoReceived != null)
                WorkspaceInfoReceived(this, new WorkspaceInfoArgs() { WorkspaceInfo = workspaces });
        }
        protected virtual void OnWorkspaceLoadError(string id)
        {
            if (WorkspaceLoadError != null)
                WorkspaceLoadError(this, new WorkspaceEventArgs { UpdatedWorkspace = new QWorkspace(id) });
        }

    }
}
