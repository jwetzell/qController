using System;
namespace qController.QItems
{
    public class NoQCueSelected : QCue
    {
        public NoQCueSelected()
        {
            listName = "No Cue Selected";
            type = "";
            notes = "Workspace has loaded but no cue is selected";
            number = "!";
        }
    }
}
