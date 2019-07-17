using System.Collections.Generic;

namespace qController
{
    public class QCue
    {
        //info loaded from "/cueLists"
        public string number { get; set; }
        public string uniqueID { get; set; }
        public bool flagged { get; set; }
        public string listName { get; set; }
        public string type { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
        public bool armed { get; set; }

        //info loaded from "/valuesForKeys
        public decimal translationX { get; set; }
        public bool isRunning { get; set; }
        public decimal scaleX { get; set; }
        public bool isPaused { get; set; }
        public decimal translationY { get; set; }
        public decimal preWait { get; set; }
        public decimal opacity { get; set; }
        public decimal scaleY { get; set; }
        public decimal duration { get; set; }
        public decimal postWait { get; set; }
        public bool isBroken { get; set; }
        public string displayName { get; set; }
        public bool isLoaded { get; set; }
        public string notes { get; set; }
        public List<List<double>> levels { get; set; }

        //info loaded from /children
        public List<QCue> cues { get; set; }

        //get string for icon font from cue type
        public string getIconString()
        {
            return QIcon.GetIconFromType(type);
        }
    }
}
