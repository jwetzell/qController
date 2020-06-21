using System;
using SharpOSC;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using qController.QItems;
using Serilog;

namespace qController.Communication
{
    

    public class QParser
    {
        public QParser()
        {
           
        }

        
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
                        QOldWorkspace workspace = JsonConvert.DeserializeObject<QOldWorkspace>(msg.Arguments[0].ToString());
                        OnWorkspaceUpdated(workspace);
                    }
                    catch (Exception ex)
                    {
                        Log.Debug($"QPARSER - Workspace Load Error: {ex.ToString()}");
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

        protected virtual void OnWorkspaceUpdated(QOldWorkspace workspace)
        {
            WorkspaceUpdated?.Invoke(this, new WorkspaceEventArgs() { UpdatedWorkspace = workspace });
        }
        protected virtual void OnSelectedCueUpdated(QCue cue)
        {
            SelectedCueUpdated?.Invoke(this, new CueEventArgs() { Cue = cue });
        }

        protected virtual void OnCueInfoUpdated(QCue cue)
        {
            CueInfoUpdated?.Invoke(this, new CueEventArgs() { Cue = cue });
        }

        protected virtual void OnPlaybackPositionUpdated(string id)
        {
            PlaybackPositionUpdated?.Invoke(this, new PlaybackPositionArgs() { PlaybackPosition = id });
        }
        protected virtual void OnConnectionStatusChanged(string status, string workspace_id)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectEventArgs() { Status = status, WorkspaceId = workspace_id });
        }
        protected virtual void OnChildrenUpdated(string id, List<QCue> cues)
        {
            ChildrenUpdated?.Invoke(this, new ChildrenEventArgs() { cue_id = id, children = cues });
        }
        protected virtual void OnWorkspaceDisconnect()
        {
            WorkspaceDisconnect?.Invoke(this, new EventArgs());
        }
        protected virtual void OnWorkspaceInfoReceived(List<QWorkspaceInfo> workspaces)
        {
            WorkspaceInfoReceived?.Invoke(this, new WorkspaceInfoArgs() { WorkspaceInfo = workspaces });
        }
        protected virtual void OnWorkspaceLoadError(string id)
        {
            WorkspaceLoadError?.Invoke(this, new WorkspaceEventArgs { UpdatedWorkspace = new QOldWorkspace(id) });
        }

    }
}
