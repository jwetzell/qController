// Allows the "storage" of links between cue ids and QCueGrids
// a little hacky but works with the recursive nature of QCueGrids
// used mainly for scrolling to the appropriate QCueGrid when selected cue changes

using System.Collections.Generic;

namespace qController.UI
{
    public class QCueGridListHelper
    {
        private static Dictionary<string, QCueGrid> cueGridDict;
        static QCueGridListHelper()
        {
            cueGridDict = new Dictionary<string, QCueGrid>();
        }

        public static void reset()
        {
            cueGridDict.Clear();
        }

        public static void insert(string cueID, QCueGrid cueGrid)
        {
            if (cueGridDict.ContainsKey(cueID))
            {
                cueGridDict[cueID] = cueGrid;
            }
            else
            {
                cueGridDict.Add(cueID, cueGrid);
            }
        }

        public static QCueGrid get(string cueID)
        {
            if (cueGridDict.ContainsKey(cueID))
            {
                return cueGridDict[cueID];
            }
            else {
                return null;
            }
        }
    }
}
