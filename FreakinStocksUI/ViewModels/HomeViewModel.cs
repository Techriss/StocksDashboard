using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        private ChartValues<decimal> _prices;
        private Security _stockInfo;
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

        public Security StockInfo
        {
            get => _stockInfo;
            set
            {
                _stockInfo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(PriceChange));
                OnPropertyChanged(nameof(MarketCap));
                OnPropertyChanged(nameof(MarketCapChange));
            }
        }

        public double Price => StockInfo?.RegularMarketPrice ?? 0;
        public ValueChange PriceChange => new($"{ Math.Round(StockInfo?.RegularMarketChangePercent ?? 0, 2) }%", GetColorForValue(StockInfo? .RegularMarketChangePercent ?? 0));
        public string MarketCap => $"{StockInfo?.MarketCap:#.###M}";
        public ValueChange MarketCapChange => new($"{ Math.Round(StockInfo?.PostMarketChangePercent ?? 0, 2) }%", GetColorForValue(StockInfo?.PostMarketChangePercent ?? 0));




        private async Task LoadPrices()
        {
            var data = await StocksData.StocksDataAccess.GetLastWeek(CurrentStock);
            var info = await StocksData.StocksDataAccess.GetStockData(CurrentStock);

            Prices = new(data);
            StockInfo = info;
        }

        private static SolidColorBrush GetColorForValue(double value)
        {
            return value < 0 ? new(Colors.IndianRed) : new(Colors.ForestGreen);
        }




        public HomeViewModel(Page page)
        {
            Source = page;

            Task.Run(LoadPrices);
        }
    }


    public record ValueChange(string Change, SolidColorBrush Color);
}
