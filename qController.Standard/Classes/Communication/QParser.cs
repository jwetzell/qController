using System;
using SharpOSC;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace qController
{
    public class CueEventArgs : EventArgs
    {
        public QCue SelectedCue
        {
            get;
            set;
        }
    }

    public class AudioLevelArgs : EventArgs
    {
        public JToken levels
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
    public class QParser
    {
        public QParser()
        {
            
        }

        public delegate void SelectedCueUpdatedHandler(object source, CueEventArgs args);
        public delegate void AudioLevelsUpdatedHandler(object source, AudioLevelArgs args);
        public delegate void WorkspaceUpdatedHandler(object source, WorkspaceEventArgs args);

        public event SelectedCueUpdatedHandler SelectedCueUpdated;
        public event AudioLevelsUpdatedHandler AudioLevelsUpdated;
        public event WorkspaceUpdatedHandler WorkspaceUpdated;

        public void ParseMessage(OscMessage msg){
            if (!msg.Address.Contains("null"))
            {
                if (msg.Address.Contains("valuesForKeys"))
                    ParseSelectedCueInfo(msg);
                else if (msg.Address.Contains("notes"))
                    ParseNoteInfo(msg);
                else if (msg.Address.Contains("levels"))
                    ParseLevelInfo(msg);
                else if (msg.Address.Contains("cueLists"))
                    ParseWorkspaceInfo(msg);
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
        public void ParseSelectedCueInfo(OscMessage msg){
            JToken selectedCue = OSC2JSON(msg);
            Console.WriteLine("Selected Cue Updated");
            QCue cue = JsonConvert.DeserializeObject<QCue>(selectedCue.ToString());
            OnSelectedCueUpdated(cue);
        }

        public void ParseNoteInfo(OscMessage msg){
            //JToken cueInfo = OSC2JSON(msg);
        }

        public void ParseLevelInfo(OscMessage msg){
            JToken levels = OSC2JSON(msg)[0];
            OnAudioLevelsUpdated(levels);
        }

        public void ParseWorkspaceInfo(OscMessage msg)
        {
            if (msg.Arguments.Count > 0)
            {
                QWorkSpace workspace = JsonConvert.DeserializeObject<QWorkSpace>(msg.Arguments[0].ToString());
                OnWorkspaceUpdated(workspace);
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
                SelectedCueUpdated(this, new CueEventArgs() { SelectedCue = cue });
        }

        protected virtual void OnAudioLevelsUpdated(JToken audioLevels){
            if (AudioLevelsUpdated != null)
                AudioLevelsUpdated(this, new AudioLevelArgs { levels = audioLevels });
        }
    }
}
