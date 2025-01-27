using System;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

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
            MenuPage = new MenuPage();
            MenuPage.setItemSelected(OnItemSelected);

            Flyout = MenuPage;
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MenuPageItem;
            if (item != null)
            {
                MenuPage.clearSelectedItem();
                IsPresented = false;
                MenuItemSelected?.Invoke(this, new MenuEventArgs { Command = item.Command });
            }
        }
    }
}
