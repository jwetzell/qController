using System.Collections.Generic;

namespace qController
{
    public class QCueList
    {
        public string number { get; set; }
        public string uniqueID { get; set; }
        public List<QCue> cues { get; set; }
        public bool flagged { get; set; }
        public string listName { get; set; }
        public string type { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
        public bool armed { get; set; }
    }
}
