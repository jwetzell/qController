namespace qController
{
    public static  class QCommands
    {
        public static QCommand GO = new QCommand("GO", "/go","WORKSPACE");
        public static QCommand PANIC = new QCommand("Panic", "/panic","WORKSPACE");
        public static QCommand PAUSE = new QCommand("Pause", "/pause","WORKSPACE");
        public static QCommand PREVIEW = new QCommand("Preview", "/preview","WORKSPACE");
        public static QCommand RESUME = new QCommand("Resume", "/resume","WORKSPACE");
        public static QCommand PREVIOUS = new QCommand("Previous","/select/previous","WORKSPACE");
        public static QCommand NEXT = new QCommand("Next", "/select/next","WORKSPACE");
        public static QCommand RESET = new QCommand("Reset","/reset","WORKSPACE");
        public static QCommand STOP = new QCommand("Stop","/stop","WORKSPACE");
        public static QCommand HARDSTOP = new QCommand("Hard Stop", "/hardStop","WORKSPACE");

    }
}
