using System;
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

        public const string SPIN3 = "\uE832";

        public const string MENU = "\uF0C9";

        public const string LIGHTBULB = "\uF0CB";

        public const string MIC = "\uF130";

        public const string MINUS_SQUARED = "\uF146";

        public const string RIGHT = "\uF178";

        public const string DOT_CIRCLED = "\uF192";

        public const string SLIDERS = "\uF1DE";

        public const string WIFI = "\uF1EB";

        public const string OBJECT_UNGROUP = "\uF248";

        public const string HOURGLASS_O = "\uF251";

        public const string COMMENTING_O = "\uF27B";

        public const string DOLLAR = "\uF155";
        public const string MAIL = "\uE812";


        public static string GetIconFromType(string type)
        {
            switch (type)
            {
                case "Group":
                    return OBJECT_UNGROUP;
                case "Audio":
                    return VOLUME_UP;
                case "Mic":
                    return MIC;
                case "Video":
                    return VIDEO;
                case "Camera":
                    return VIDEOCAM;
                case "Text":
                    return "T";
                case "Light":
                    return LIGHTBULB;
                case "Fade":
                    return SLIDERS;
                case "Network":
                    return DOT_CIRCLED;
                case "MIDI":
                    return "";
                case "MIDI File":
                    return MUSIC;
                case "Timecode":
                    return CLOCK;
                case "Start":
                    return PLAY;
                case "Stop":
                    return STOP;
                case "Pause":
                    return PAUSE;
                case "Load":
                    return CHART_PIE;
                case "Reset":
                    return UNDO;
                case "Devamp":
                    return CW;
                case "GoTo":
                    return RIGHT;
                case "Target":
                    return TARGET;
                case "Arm":
                    return POWER;
                case "Disarm":
                    return "";
                case "Wait":
                    return HOURGLASS_O;
                case "Memo":
                    return COMMENTING_O;
                case "Script":
                    return QRCODE;
                default:
                    return "";
            }
        }
    }
}
