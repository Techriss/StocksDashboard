using System.Windows.Controls;

namespace FreakinStocksUI.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        public HomeViewModel(Page page)
        {
            Source = page;
        }
    }
}
