//IP Address verification
//IMPLEMENT A ISREACHABLE METHOD TO BE ABLE TO DISPLAY ONLINE QINSTANCES
using System.Net;

namespace qController.Communication
{
    public class IPHelper
    {
        public IPHelper()
        {
        }

        public static bool IsValidAddress(string ipString){
            if ((ipString.Split('.').Length - 1) != 3) return false;
            IPAddress address;
            return IPAddress.TryParse(ipString, out address);
        }
    }
}
