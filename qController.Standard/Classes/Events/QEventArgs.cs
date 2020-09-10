using System;
namespace qController.Events
{
    public class CueEditArgs : EventArgs
    {
        public string CueID
        {
            get;
            set;
        }
        public string Property
        {
            get;
            set;
        }
        public string NewValue
        {
            get;
            set;
        }
    }
}
