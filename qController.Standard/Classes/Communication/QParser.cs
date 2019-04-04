using System;
using SharpOSC;
using Newtonsoft.Json.Linq;

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

    public class QParser
    {
        public QParser()
        {
            
        }

        public delegate void SelectedCueUpdatedHandler(object source, CueEventArgs args);
        public delegate void AudioLevelsUpdatedHandler(object source, AudioLevelArgs args);

        public event SelectedCueUpdatedHandler SelectedCueUpdated;
        public event AudioLevelsUpdatedHandler AudioLevelsUpdated;

        public void ParseMessage(OscMessage msg){
            if (msg.Address.Contains("selectedCues"))
                ParseSelectedCueInfo(msg);
            else if (msg.Address.Contains("notes"))
                ParseNoteInfo(msg);
            else if (msg.Address.Contains("levels"))
                ParseLevelInfo(msg);

        }

        public void OnMessageReceived(object source, MessageEventArgs args){
            //Console.WriteLine("Packet Received");
            ParseMessage(args.Message);
        }

        public void ParseSelectedCueInfo(OscMessage msg){
            JArray selectedCues = (JArray)OSC2JSON(msg);
            if(selectedCues.Count > 0){
                foreach(JObject item in selectedCues){
                    QCue parsedCue = new QCue(item);
                    OnSelectedCueUpdated(parsedCue);
                }

            }else{
                Console.WriteLine("No Cues Selected");
            }
        }

        public void ParseNoteInfo(OscMessage msg){
            //JToken cueInfo = OSC2JSON(msg);
        }

        public void ParseLevelInfo(OscMessage msg){
            JToken levels = OSC2JSON(msg)[0];
            OnAudioLevelsUpdated(levels);
        }

        public JToken OSC2JSON(OscMessage Msg){
            JObject json = JObject.Parse(Msg.Arguments.ToArray()[0].ToString());
            return json.GetValue("data");
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
