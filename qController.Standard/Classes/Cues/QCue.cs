using System;
using Newtonsoft.Json.Linq;
namespace qController
{
    public class QCue
    {
        public string number { get; set; }
        public string uniqueID { get; set; }
        public bool flagged { get; set; }
        public string listName { get; set; }
        public string type { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
        public bool armed { get; set; }
        public int translationX { get; set; }
        public bool isRunning { get; set; }
        public int scaleX { get; set; }
        public bool isPaused { get; set; }
        public int translationY { get; set; }
        public int preWait { get; set; }
        public int opacity { get; set; }
        public int scaleY { get; set; }
        public int duration { get; set; }
        public int postWait { get; set; }
        public bool isBroken { get; set; }
        public string displayName { get; set; }
        public bool isLoaded { get; set; }
        public string notes { get; set; }

        public string getIconString()
        {
            switch (type)
            {
                case "Group":
                    return "\uF248";
                case "Audio":
                    return "\uE807";
                case "Mic":
                    return "\uF130";
                case "Video":
                    return "\uE805";
                case "Camera":
                    return "\uE806";
                case "Text":
                    return "T";
                case "Light":
                    return "\uF0EB";
                case "Fade":
                    return "\uF1DE";
                case "Network":
                    return "\uF192";
                case "MIDI":
                    return "";
                case "MIDI File":
                    return "\uE804";
                case "Timecode":
                    return "\uE808";
                case "Start":
                    return "\uE809";
                case "Stop":
                    return "\uE80A";
                case "Pause":
                    return "\uE80B";
                case "Load":
                    return "\uE80E";
                case "Reset":
                    return "\uE80F";
                case "Devamp":
                    return "\uE80C";
                case "GoTo":
                    return "\uF178";
                case "Target":
                    return "\uE80D";
                case "Arm":
                    return "\uE810";
                case "Disarm":
                    return "";
                case "Wait":
                    return "\uF250";
                case "Memo":
                    return "\uF27B";
                case "Script":
                    return "\uE811";
                default:
                    return "";
            }
        }
    }
}
