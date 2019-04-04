using System;
using Xamarin.Forms;
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
                qController.sendCommandUDP("/selectedCues");
                qController.sendCommandUDP("/cue/selected/levels");
                return active;
            });
        }

        public void Kill(){
            active = false;
        }
    }
}
