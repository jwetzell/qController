using System;
namespace qController
{
    public class QCommand
    {
        public string osc;
        public string text;
        public string type;
        
        public QCommand(string display, string cmd, string command_type)
        {
            text = display;
            osc = cmd;
            type = command_type;

        }
    }
}
