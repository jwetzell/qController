using System;
using Newtonsoft.Json.Linq;
namespace qController
{
    public class QCue
    {
        public string number;
        public string uniqueID;
        public bool flagged;
        public string listName;
        public string type;
        public string colorName;
        public string name;
        public bool armed;

        public QCue(JObject json){
            //Console.WriteLine(json);
            number = (string)json.GetValue("number");
            uniqueID = (string)json.GetValue("uniqueID");
            flagged = (bool)json.GetValue("flagged");
            listName = (string)json.GetValue("listName");
            type = (string)json.GetValue("type");
            colorName = (string)json.GetValue("colorName");
            name = (string)json.GetValue("name");
            armed = (bool)json.GetValue("armed");
        }

    }
}
