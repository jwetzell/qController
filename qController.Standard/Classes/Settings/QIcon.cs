//Get appropriately letter or unicode character for Icon font
namespace qController
{
    public static class QIcon
    {
        public const string SEARCH = "\uE800";
        public const string PLUS = "\uE801";
        public const string TRASH_EMPTY = "\uE802";
        public const string CANCEL = "\uE803";
        public const string MUSIC = "\uE804";
        public const string VIDEO = "\uE805";
        public const string VIDEOCAM = "\uE806";
        public const string VOLUME_UP = "\uE807";
        public const string CLOCK = "\uE808";
        public const string PLAY = "\uE809";
        public const string STOP = "\uE80A";
        public const string PAUSE = "\uE80B";
        public const string CW = "\uE80C";
        public const string TARGET = "\uE80D";
        public const string CHART_PIE = "\uE80E";
        public const string UNDO = "\uE80F";

        public const string POWER = "\uE810";
        public const string QRCODE = "\uE811";


        public const string MAIL = "\uE812";

        public const string LEFT_DIR = "\uE813";
        public const string LIST = "\uE814";

        public const string SPIN3 = "\uE832";

        public const string MENU = "\uF0C9";

        public const string LIGHTBULB = "\uF0EB";

        public const string MIC = "\uF130";

        public const string MINUS_SQUARED = "\uF146";

        public const string DOLLAR = "\uF155";

        public const string RIGHT = "\uF178";

        public const string DOT_CIRCLED = "\uF192";

        public const string SLIDERS = "\uF1DE";

        public const string WIFI = "\uF1EB";

        public const string OBJECT_UNGROUP = "\uF248";

        public const string HOURGLASS_O = "\uF250";

        public const string COMMENTING_O = "\uF27B";


        public static string GetIconFromType(string type)
        {
            switch (type)
            {
                case "Group":
                    return "g";
                case "Audio":
                    return "a";
                case "Mic":
                    return "m";
                case "Video":
                    return "v";
                case "Camera":
                    return "c";
                case "Text":
                    return "t";
                case "Light":
                    return LIGHTBULB;
                case "Fade":
                    return "f";
                case "Network":
                    return "o";
                case "MIDI":
                    return "M";
                case "MIDI File":
                    return "F";
                case "Timecode":
                    return "T";
                case "Start":
                    return "s";
                case "Stop":
                    return "S";
                case "Pause":
                    return "p";
                case "Load":
                    return "l";
                case "Reset":
                    return "r";
                case "Devamp":
                    return "d";
                case "GoTo":
                    return "G";
                case "Target":
                    return "R";
                case "Arm":
                    return "A";
                case "Disarm":
                    return "D";
                case "Wait":
                    return "W";
                case "Memo":
                    return "e";
                case "Script":
                    return "C";
                default:
                    return "";
            }
        }
    }
}
