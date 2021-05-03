using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveCharts;

namespace FreakinStocksUI.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        private ChartValues<decimal> _prices;
        private string _currentStock = "TSLA";


        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                _currentStock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentDisplayStock));
            }
        }

        public string CurrentDisplayStock => string.Join(" ", CurrentStock.ToCharArray());


        public ChartValues<decimal> Prices
        {
            get => _prices;
            set
            {
                _prices = value;
                OnPropertyChanged();
            }
        }




        private async Task LoadPrices()
        {
            var data = await StocksData.StocksDataAccess.GetLastWeek(CurrentStock);
            Prices = new(data);
        }




        public HomeViewModel(Page page)
        {
            Source = page;

            Task.Run(LoadPrices);
        }
    }
}
