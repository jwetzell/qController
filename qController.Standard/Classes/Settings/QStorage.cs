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
        public static QRecentWorkspaceInfo recentWorkspaceInfo;

        static QStorage(){
            
            qInstances = (ObservableCollection<QInstance>)CrossSettings.Current.GetValue(new ObservableCollection<QInstance>().GetType(), "qInstances", null);
            recentWorkspaceInfo = (QRecentWorkspaceInfo)CrossSettings.Current.GetValue(typeof (QRecentWorkspaceInfo),"recentWorkspaceInfo",null);
            if (qInstances == null)
            {
                Log.Debug("QSTORAGE - No qInstance exists creating one");
                qInstances = new ObservableCollection<QInstance>();
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
