using System.Windows.Controls;
namespace FreakinStocksUI.ViewModels
{
    class LiveViewModel : ViewModelBase
    {
        public LiveViewModel(Page page)
        {
            Source = page;
        }
    }
}
