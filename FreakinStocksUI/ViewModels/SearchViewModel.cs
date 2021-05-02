using System.Windows.Controls;

namespace FreakinStocksUI.ViewModels
{
    class SearchViewModel : ViewModelBase
    {
        public SearchViewModel(Page page)
        {
            Source = page;
        }
    }
}
