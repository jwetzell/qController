using System;
using System.Threading;
using Xamarin.Forms;
using SharpOSC;
namespace qController
{
    public class QUpdater
    {

        private bool active = true;
        private QController qController;
        private Thread updateThread; 
        public QUpdater(QController controller)
        {
            qController = controller;
            updateThread = new Thread(new ThreadStart(UpdateLoop));
        }

        public void Start(){
            updateThread.Start();
        }

        public void UpdateLoop()
        {
            active = true;
            Device.StartTimer(TimeSpan.FromSeconds(0.01), () => {
                UpdateSelected();
                UpdateLevels();
                return active;
            });
        }

        public void UpdateSelected()
        {
            string valuesForKeys = "[\"displayName\",\"number\",\"type\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"uniqueID\",\"flagged\",\"listName\",\"colorName\",\"name\",\"armed\"]";
            //qController.qClient.sendAndReceiveStringArgs("/cue/selected/valuesForKeys", valuesForKeys);
        }
        public void UpdateLevels()
        {
            qController.qClient.sendAndReceiveString("/cue/selected/levels");
        }

        public void Kill(){
            active = false;
            updateThread.Abort();
        }
    }
}
