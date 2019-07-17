using System;
using Xamarin.Forms;

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
        public MenuPage MenuPage;
        public delegate void MenuItemSelectedHandler(object source, MenuEventArgs args);
        public event MenuItemSelectedHandler MenuItemSelected;

        public RootPage()
        {
            InitializeComponent();

        }

        public void Init()
        {
            MenuPage = (MenuPage)Master;

            MenuPage.listView.ItemSelected += OnItemSelected;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuPageItem;
            if (item != null)
            {
                MenuPage.listView.SelectedItem = null;
                IsPresented = false;
                if (MenuItemSelected != null)
                    MenuItemSelected(this, new MenuEventArgs { Command = item.Command });
            }
        }
    }
}
