namespace qController.QItems
{
    public class QInstance
    {
        public string name { get; set; }
        public string address { get; set; }
        public QInstance()
        {

        }
        public QInstance(string tName, string tAddress)
        {
            name = tName;
            address = tAddress;
        }
        public bool IsReachable()
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