using System;
using Xamarin.Forms;

namespace qController
{
    public class QButton : Button
    {
        public string OSCCommand
        {
            get;
            set;
        }
        public QButton(string text, string cmd)
        {
            Text = text;
            OSCCommand = cmd;
        }

    }
}
