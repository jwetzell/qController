using System;
using Xamarin.Forms;
using SharpOSC;
namespace qController
{
    public class QUpdater
    {
        private bool active = true;
        private QController qController;
        public QUpdater(QController controller)
        {
            qController = controller;
        }

        public void Start(){
            active = true;
            Device.StartTimer(TimeSpan.FromSeconds(0.01), () => {
                //qController.sendCommandUDP("/selectedCues");
                UpdateSelected();
                UpdateLevels();
                return active;
            });

        }

        public void UpdateSelected()
        {
            string valuesForKeys = "[\"displayName\",\"number\",\"type\",\"isBroken\",\"isLoaded\",\"isPaused\",\"isRunning\",\"preWait\",\"duration\",\"postWait\",\"translationX\",\"translationY\",\"opacity\",\"scaleX\",\"scaleY\",\"uniqueID\",\"flagged\",\"listName\",\"colorName\",\"name\",\"armed\"]";
            qController.qClient.sendAndReceiveStringArgs("/cue/selected/valuesForKeys", valuesForKeys);
        }
        public void UpdateLevels()
        {
            qController.qClient.sendAndReceiveString("/cue/selected/levels");
        }

        public void Kill(){
            active = false;
        }
    }
}
