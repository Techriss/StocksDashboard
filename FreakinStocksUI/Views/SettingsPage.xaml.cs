using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            this.DataContext = new SettingsViewModel(this);
        }
    }
}
