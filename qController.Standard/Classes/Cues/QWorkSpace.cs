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
    }
}
