//IP Address verification
//IMPLEMENT A ISREACHABLE METHOD TO BE ABLE TO DISPLAY ONLINE QINSTANCES
using System.Net;

namespace qController.Helpers
{
    public class IPHelper
    {
        public IPHelper()
        {
        }

        public static bool IsValidAddress(string ipString){
            return (ipString.Split('.').Length - 1) == 3 && IPAddress.TryParse(ipString, out _);
        }

        public static bool IsReachable(string ipString)
        {
            /*try
            {
                IPAddress instanceIP = IPAddress.Parse(address);
                Ping p = new System.Net.NetworkInformation.Ping();
                PingReply reply = p.Send(instanceIP);
                return reply.Status == IPStatus.Success ? true : false;
            }
            catch
            {
                return false;
            }*/
            return true;
        }
    }
}
