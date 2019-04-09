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
    }
}
