using Serilog;
using System.Collections.ObjectModel;
using Acr.Settings;
using qController.QItems;

namespace qController
{
    public class QStorage
    {
        public static ObservableCollection<QInstance> qInstances;
        static QStorage(){
            
            qInstances = (ObservableCollection<QInstance>)CrossSettings.Current.GetValue(new ObservableCollection<QInstance>().GetType(), "qInstances", null);
            if (qInstances == null)
            {
                Log.Debug("QSTORAGE - No qInstance exists creating one");
                qInstances = new ObservableCollection<QInstance>();
                updateStorage();
            }
        }

        public static bool AddInstance(string name, string address){
            return AddInstance(new QInstance(name, address));
        }

        public static bool AddInstance(QInstance q){
            if(!Contains(q.name, q.address)){
                qInstances.Add(q);
                updateStorage();
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
            updateStorage();
        }

        private static void updateStorage(){
            CrossSettings.Current.SetValue("qInstances", qInstances);
        }

        public static bool Contains(string name, string address){
            foreach (var item in qInstances)
            {
                Log.Information("Item name: " + item.name + " Found Name: " + name);
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
