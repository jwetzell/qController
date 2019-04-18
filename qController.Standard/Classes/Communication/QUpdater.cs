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
            Device.StartTimer(TimeSpan.FromSeconds(5.0), () => {
                SendThump();
                return active;
            });
        }

        public void SendThump()
        {
            qController.qClient.sendStringUDP("/thump");
        }

        public void Kill(){
            active = false;
            updateThread.Abort();
        }
    }
}
