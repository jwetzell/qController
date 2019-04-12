using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace qController
{
    public class MenuEventArgs : EventArgs
    {
        public string Command
        {
            get;
            set;
        }
    }

    public partial class RootPage : MasterDetailPage
    {
        MenuPage MenuPage;
        public delegate void MenuItemSelectedHandler(object source, MenuEventArgs args);
        public event MenuItemSelectedHandler MenuItemSelected;

        public RootPage()
        {
            InitializeComponent();

        }

        public void Init()
        {
            MenuPage = (MenuPage)Master;

            MenuPage.ListView.ItemSelected += OnItemSelected;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                MenuPage.ListView.SelectedItem = null;
                IsPresented = false;
                if (MenuItemSelected != null)
                    MenuItemSelected(this, new MenuEventArgs { Command = item.Command });
            }
        }
    }
}
