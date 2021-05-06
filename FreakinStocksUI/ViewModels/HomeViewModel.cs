using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using FreakinStocksUI.Models;
using LiveCharts;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        #region private

        private ChartValues<decimal> _prices;
        private List<string> _dates;
        private Security _stockInfo;
        private string _currentStock;
        private int _currentIndex = 0;

        #endregion



        #region public

        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                _currentStock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentDisplayStock));

                Task.Run(LoadPrices);
            }
        }
        public string CurrentDisplayStock => CurrentStock is null ? "" : string.Join(" ", CurrentStock?.ToCharArray());

        public ChartValues<decimal> Prices
        {
            get => _prices;
            set
            {
                _prices = value;
                OnPropertyChanged();
            }
        }
        public List<string> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
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
                OnPropertyChanged(nameof(PriceChange));
            }
        }
        public ValueChange PriceChange
        {
            get
            {
                return new($"{ Math.Round(StockInfo?.RegularMarketChangePercent ?? 0, 2) }%", ValueChange.GetColorForValue(StockInfo?.RegularMarketChangePercent ?? 0));
            }
        }

        public string[] Stocks { get; set; } = Properties.Settings.Default.LikedStocks?.ToArray() ?? new[] { "TSLA", "NDAQ", "AAPL" };
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));

                CurrentStock = Stocks[value];
            }
        }
        public bool CanGoNext => CurrentIndex + 1 <= Stocks.Length - 1;
        public bool CanGoPrevious => CurrentIndex - 1 >= 0;


        #endregion



        #region commands

        public RelayCommand GoNext => new(() => CurrentIndex++);

        public RelayCommand GoPrevious => new(() => CurrentIndex--);

        public RelayCommand ReloadCommand => new(async () => await LoadPrices());

        #endregion



        #region methods

        private async Task LoadPrices()
        {
            Stocks = GetStocks();
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanGoPrevious));
            var data = (await StockMarketData.GetLastWeek(CurrentStock)).ToList();
            var prices = data.Select(x => x.Price);
            var dates = data.Select(x => $"{DateTime.Parse(x.Time):dddd}").ToList();
            var info = await StockMarketData.GetStockData(CurrentStock);

            Prices = new(prices);
            Dates = dates;
            StockInfo = info;
        }

        private string[] GetStocks()
        {
            return Properties.Settings.Default.LikedStocks?.ToArray() ?? new[] { "TSLA", "NDAQ", "AAPL" };
        }

        #endregion


        public HomeViewModel(Page page)
        {
            Source = page;
            CurrentStock = Stocks[CurrentIndex];

            Task.Run(LoadPrices);
        }
    }
}
