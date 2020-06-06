//Class pertains to the local copy of a workspace, with methods for fetching/updating information inside

using System;
using System.Collections.Generic;
using Serilog;

namespace qController
{
    public class QWorkspace
    {
        public string status { get; set; }
        public List<QCueList> data { get; set; }
        public string workspace_id { get; set; }
        public string address { get; set; }
        public bool IsPopulated { get; set; }

        //ONLY FOR PASSING A WORKSPACE LOAD ERROR
        public QWorkspace(string id)
        {
            workspace_id = id;
        }

        public QCue GetCue(string cue_id)
        {
            foreach (var cueList in data)
            {
                foreach (var cue in cueList.cues)
                {
                    if(cue.uniqueID == cue_id)
                    {
                        return cue;
                    }
                }
            }
            return null;
        }

        public QCueList GetCueList(string id)
        {
            foreach (var cueList in data)
            {
                if(cueList.uniqueID == id)
                {
                    return cueList;
                }
            }
            return null;
        }

        public void UpdateCue(QCue cue)
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].cues.Count; j++)
                {
                    if(data[i].cues[j].uniqueID == cue.uniqueID)
                    {
                        //Log.Debug("Cue found and updated in workspace: " + cue.uniqueID);
                        data[i].cues[j] = cue;
                        return;
                    }
                }
            }
        }

        public void UpdateChildren(string cue_id, List<QCue> children)
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].cues.Count; j++)
                {
                    if (data[i].cues[j].uniqueID == cue_id)
                    {
                        if(data[i].cues[j].type == "Group")
                        {
                            data[i].cues[j].cues = children;
                            return;
                        }
                    }
                }
            }
        }

        public bool CheckPopulated()
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].cues.Count; j++)
                {
                    if (data[i].cues[j].type == "Group" && data[i].cues[j].cues == null)
                    {
                        IsPopulated = false;
                        return false;
                    }
                }
            }
            IsPopulated = true;
            return true;
        }

        public void PrintStats()
        {
            foreach (var cueList in data)
            {
                Log.Debug(cueList.listName + "("+ cueList.cues.Count + " cues)");
            }
        }
    }
}
