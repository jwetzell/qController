using System;
using SharpOSC;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

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

    public class AudioLevelArgs : EventArgs
    {
        public string cue_id
        {
            get;
            set;
        }
        public List<double> levels
        {
            get;
            set;
        }
    }
    public class WorkspaceEventArgs : EventArgs
    {
        public QWorkSpace UpdatedWorkspace
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
        public delegate void AudioLevelsUpdatedHandler(object source, AudioLevelArgs args);
        public delegate void WorkspaceUpdatedHandler(object source, WorkspaceEventArgs args);
        public delegate void CueInfoUpdatedHandler(object source, CueEventArgs args);
        public delegate void PlaybackPositionUpdatedHandler(object source, PlaybackPositionArgs args);
        public delegate void ConnectionStatusHandler(object source, ConnectEventArgs args);
        public delegate void ChildrenUpdateHandler(object source, ChildrenEventArgs args);
        public event ChildrenUpdateHandler ChildrenUpdated;
        public event ConnectionStatusHandler ConnectionStatusChanged;
        public event SelectedCueUpdatedHandler SelectedCueUpdated;
        public event AudioLevelsUpdatedHandler AudioLevelsUpdated;
        public event WorkspaceUpdatedHandler WorkspaceUpdated;
        public event CueInfoUpdatedHandler CueInfoUpdated;
        public event PlaybackPositionUpdatedHandler PlaybackPositionUpdated;

        public void ParseMessage(OscMessage msg){
            if (!msg.Address.Contains("null"))
            {
                if (msg.Address.Contains("valuesForKeys"))
                {
                    if (msg.Address.Contains("WithArguments"))
                        ParseLevelInfo(msg);
                    else
                        ParseCueUpdateInfo(msg);
                }
                else if (msg.Address.Contains("notes"))
                    ParseNoteInfo(msg);
                else if (msg.Address.Contains("levels"))
                    ParseLevelInfo(msg);
                else if (msg.Address.Contains("cueLists"))
                    ParseWorkspaceInfo(msg);
                else if (msg.Address.Contains("playbackPosition"))
                    ParsePositionUpdateInfo(msg);
                else if (msg.Address.Contains("thump"))
                    Console.WriteLine("Heartbeat Received");
                else if (msg.Address.Contains("connect"))
                    ParseConnectInfo(msg);
                else if (msg.Address.Contains("children"))
                    ParseChildrenInfo(msg);
                else
                {
                    Console.WriteLine("Unknown message type");
                    Console.WriteLine(msg.Address);
                    foreach (var item in msg.Arguments)
                    {
                        Console.WriteLine(item);
                    }
                }
            }
        }
        
        public void ParseConnectInfo(OscMessage msg)
        {
            JToken connectStatus = OSC2JSON(msg);
            OnConnectionStatusChanged(connectStatus.ToString());
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

        public void ParseNoteInfo(OscMessage msg){
            //JToken cueInfo = OSC2JSON(msg);
        }

        public void ParseLevelInfo(OscMessage msg){
            var cue_id = msg.Address.Split('/')[3];
            JToken levels = OSC2JSON(msg)[0];
            OnAudioLevelsUpdated(cue_id, levels.ToObject<List<double>>());
        }

        public void ParseWorkspaceInfo(OscMessage msg)
        {
            if (msg.Arguments.Count > 0)
            {
                QWorkSpace workspace = JsonConvert.DeserializeObject<QWorkSpace>(msg.Arguments[0].ToString());
                OnWorkspaceUpdated(workspace);
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

        protected virtual void OnWorkspaceUpdated(QWorkSpace workspace)
        {
            if (WorkspaceUpdated != null)
                WorkspaceUpdated(this, new WorkspaceEventArgs() { UpdatedWorkspace = workspace });
        }
        protected virtual void OnSelectedCueUpdated(QCue cue)
        {
            if (SelectedCueUpdated != null)
                SelectedCueUpdated(this, new CueEventArgs() { Cue = cue });
        }

        protected virtual void OnAudioLevelsUpdated(string id, List<double> audioLevels){
            if (AudioLevelsUpdated != null)
                AudioLevelsUpdated(this, new AudioLevelArgs { cue_id = id, levels = audioLevels });
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
        protected virtual void OnConnectionStatusChanged(string status)
        {
            if (ConnectionStatusChanged != null)
                ConnectionStatusChanged(this, new ConnectEventArgs() { Status = status });
        }
        protected virtual void OnChildrenUpdated(string id, List<QCue> cues)
        {
            if (ChildrenUpdated != null)
                ChildrenUpdated(this, new ChildrenEventArgs() { cue_id = id, children = cues });
        }

    }
}
