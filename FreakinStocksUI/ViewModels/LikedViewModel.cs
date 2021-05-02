using System.Windows.Controls;
namespace FreakinStocksUI.ViewModels
{
    class LikedViewModel : ViewModelBase
    {
        public LikedViewModel(Page page)
        {
            Source = page;
        }
    }
}
