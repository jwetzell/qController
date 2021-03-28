using Serilog;
using System.Collections.ObjectModel;
using Acr.Settings;
using qController.QItems;
using QControlKit;

namespace qController
{
    public class QStorage
    {
        public static ObservableCollection<QInstance> qInstances;
        public static ObservableCollection<QServerInfo> qStoredServers;
        public static QRecentWorkspaceInfo recentWorkspaceInfo;

        static QStorage(){
            
            qInstances = (ObservableCollection<QInstance>)CrossSettings.Current.GetValue(new ObservableCollection<QInstance>().GetType(), "qInstances", null);
            qStoredServers = (ObservableCollection<QServerInfo>)CrossSettings.Current.GetValue(new ObservableCollection<QInstance>().GetType(), "qStoredServers", null);
            recentWorkspaceInfo = (QRecentWorkspaceInfo)CrossSettings.Current.GetValue(typeof (QRecentWorkspaceInfo),"recentWorkspaceInfo",null);

            if (qInstances == null)
            {
                Log.Debug("[QStorage] No qInstances exists creating one");
                qInstances = new ObservableCollection<QInstance>();
                UpdateStorage();
            }

            if (qStoredServers == null)
            {
                Log.Debug("[QStorage] No qStoredServers exists creating one");
                qStoredServers = new ObservableCollection<QServerInfo>();

                
                UpdateStorage();
            }

            if (qInstances != null)
            {
                Log.Debug("[QStorage] qInstances exists though so load in those instances as servers");
                foreach (QInstance instance in qInstances)
                {
                    QServerInfo serverInfo = new QServerInfo();
                    serverInfo.host = instance.address.ToString();
                    //TODO: port?
                    qStoredServers.Add(serverInfo);
                }
                UpdateStorage();
            }
        }

        public static void UpdateRecentWorkspace(QRecentWorkspaceInfo packagedInfo)
        {
            recentWorkspaceInfo = packagedInfo;
            CrossSettings.Current.SetValue("recentWorkspaceInfo", recentWorkspaceInfo);
        }

        public static bool AddInstance(string name, string address){
            return AddInstance(new QInstance(name, address));
        }

        public static bool AddInstance(QInstance q){
            if(!Contains(q.name, q.address)){
                qInstances.Add(q);
                UpdateStorage();
                return true;
            }else{
                return false;
            }
        }

        public static void RemoveInstance(string name, string address){
            foreach (var item in qInstances)
            {
                if(item.address == address && item.name == name){
                    RemoveInstance(item);
                    return;
                }
            }
        }
        public static void RemoveInstance(QInstance q){
            qInstances.Remove(q); 
            UpdateStorage();
        }

        private static void UpdateStorage(){
            CrossSettings.Current.SetValue("qInstances", qInstances);
            CrossSettings.Current.SetValue("qStoredServers", qStoredServers);
        }

        public static bool Contains(string name, string address){
            foreach (var item in qInstances)
            {
                //Log.Debug("Item name: " + item.name + " Found Name: " + name);
                if (item.address == address)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsEmpty(){
            return qInstances.Count == 0;
        }
    }
}
