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

        public QCue GetCue(string cue_id)
        {
            QCue returnCue = null;
            foreach (var cueList in data)
            {
                foreach (var cue in cueList.cues)
                {
                    if(cue.uniqueID == cue_id)
                    {
                        returnCue = cue;
                    }
                }
            }
            return returnCue;
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
                    }
                }
            }
        }

        public void UpdateLevels(string cue_id, List<double> levels)
        {
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].cues.Count; j++)
                {
                    if (data[i].cues[j].uniqueID == cue_id)
                    {
                        Console.WriteLine("Cue found and levels updated");
                        data[i].cues[j].levels = levels;
                    }
                }
            }
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
