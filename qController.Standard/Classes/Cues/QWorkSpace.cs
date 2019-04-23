using System;
using System.Collections.Generic;

namespace qController
{
    public class QWorkSpace
    {
        public string status { get; set; }
        public List<QCueList> data { get; set; }
        public string workspace_id { get; set; }
        public string address { get; set; }
        public bool IsPopulated { get; set; }
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

        public void UpdateCue(QCue cue)
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].cues.Count; j++)
                {
                    if(data[i].cues[j].uniqueID == cue.uniqueID)
                    {
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
                            CheckPopulated();
                            return;
                        }
                    }
                }
            }
        }

        public QCue GetEmptyGroup()
        {
            foreach (var cueList in data)
            {
                foreach (var cue in cueList.cues)
                {
                    if (cue.type == "Group" && cue.cues == null)
                    {
                        Console.WriteLine("Next Group Cue Found");
                        return cue;
                    }
                }
            }
            IsPopulated = true;
            return null;
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
                Console.WriteLine(cueList.listName + "("+ cueList.cues.Count + " cues)");
            }
        }
    }
}
