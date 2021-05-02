using System.Windows.Controls;

namespace FreakinStocksUI.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(Page page)
        {
            Source = page;
        }
    }
}
