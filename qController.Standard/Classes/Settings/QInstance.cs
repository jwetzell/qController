//Legacy but needed incase there are still instances stored in this way
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
            return Helpers.IPHelper.IsReachable(address);
        }
    }
}