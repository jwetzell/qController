using System;
using System.Net;

namespace qController
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
