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
            qController.qParser.ParseMessage(qController.qClient.sendAndReceiveString("/selectedCues"));
        }
        public void UpdateLevels()
        {
            qController.qParser.ParseMessage(qController.qClient.sendAndReceiveString("/cue/selected/levels"));
        }

        public void Kill(){
            active = false;
        }
    }
}
