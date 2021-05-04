using System.Windows.Controls;
namespace FreakinStocksUI.ViewModels
{
    class LiveViewModel : ViewModelBase
    {
        private string _currentStock;

        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                _currentStock = value;
                OnPropertyChanged();
            }
        }



        public LiveViewModel(Page page)
        {
            Source = page;
        }
    }
}
