using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Acr.Settings;
namespace qController
{
    public class QStorage
    {
        public static ObservableCollection<QInstance> qInstances;
        static QStorage(){
            
            qInstances = (ObservableCollection<QInstance>)CrossSettings.Current.GetValue(new ObservableCollection<QInstance>().GetType(), "qInstances", null);

            if (qInstances == null)
            {
                Console.WriteLine("No qInstance exists creating one");
                qInstances = new ObservableCollection<QInstance>();
                updateStorage();
            }
        }

        public static void AddInstance(string name, string address){
            if(!Contains(name,address)){
                qInstances.Add(new QInstance(name, address));
            }
        }
        public static void AddInstance(QInstance q){
            qInstances.Add(q);
            updateStorage();
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
                if (item.name == name && item.address == address)
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
