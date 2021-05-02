using System.Windows.Controls;
namespace FreakinStocksUI.ViewModels
{
    class AnalyticsViewModel : ViewModelBase
    {
        public AnalyticsViewModel(Page page)
        {
            Source = page;
        }
    }
}
