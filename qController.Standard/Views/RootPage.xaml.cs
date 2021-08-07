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

    public partial class RootPage : FlyoutPage
    {
        public MenuPage MenuPage;
        public delegate void MenuItemSelectedHandler(object source, MenuEventArgs args);
        public event MenuItemSelectedHandler MenuItemSelected;

        public RootPage()
        {
            InitializeComponent();
            Flyout = new MenuPage();
        }

        public void Init()
        {
            MenuPage = (MenuPage)Flyout;

            MenuPage.setItemSelected(OnItemSelected);
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is MenuPageItem item)
            {
                MenuPage.clearSelectedItem();
                IsPresented = false;
                MenuItemSelected?.Invoke(this, new MenuEventArgs { Command = item.Command });
            }
        }
    }
}
