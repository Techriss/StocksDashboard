using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    class SearchViewModel : ViewModelBase
    {
        private string _currentStock;
        private Security _stockData;

        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                value = value.ToUpper();

                if (StockMarketData.CheckSymbolExists(value) && value != _currentStock)
                {
                    _currentStock = value;
                    Task.Run(async () => await LoadData(value));
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(StockData));
                    OnPropertyChanged(nameof(DataVisibility));
                    OnPropertyChanged(nameof(TempHeaderVisibility));
                }
            }
        }

        public Security StockData
        {
            get => _stockData;
            set
            {
                _stockData = value;
                OnPropertyChanged();
            }
        }

        public Visibility TempHeaderVisibility => CurrentStock is null or "" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DataVisibility => CurrentStock is null or "" ? Visibility.Collapsed : Visibility.Visible;


        public async Task LoadData(string symbol)
        {
            var data = await StockMarketData.GetAllStockData(symbol);
            StockData = data;
            OnPropertyChanged(nameof(DataVisibility));
            OnPropertyChanged(nameof(TempHeaderVisibility));
            OnPropertyChanged(nameof(StockData));
        }

        public SearchViewModel(Page page)
        {
            Source = page;
        }
    }
}
