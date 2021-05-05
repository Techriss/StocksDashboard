using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using FreakinStocksUI.Models;
using LiveCharts;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    class HomeViewModel : ViewModelBase
    {
        private ChartValues<decimal> _prices;
        private List<string> _dates;
        private Security _stockInfo;
        private string _currentStock;
        private int _currentIndex = 0;


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
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(PriceChange));
            }
        }

        public double Price => StockInfo?.RegularMarketPrice ?? 0;
        public ValueChange PriceChange
        {
            get
            {
                return new($"{ Math.Round(StockInfo?.RegularMarketChangePercent ?? 0, 2) }%", GetColorForValue(StockInfo?.RegularMarketChangePercent ?? 0));
            }
        }

        public string[] Stocks { get; set; } = { "TSLA", "NDAQ", "AAPL" };

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



        private async Task LoadPrices()
        {
            var data = (await StockMarketData.GetLastWeek(CurrentStock)).ToList();
            var prices = data.Select(x => x.Price);
            var dates = data.Select(x => $"{DateTime.Parse(x.Time):dddd}").ToList();
            var info = await StockMarketData.GetStockData(CurrentStock);

            Prices = new(prices);
            Dates = dates;
            StockInfo = info;
        }

        private static SolidColorBrush GetColorForValue(double value)
        {
            return value < 0 ? new(Colors.IndianRed) : new(Colors.ForestGreen);
        }


        public RelayCommand GoNext => new(() => CurrentIndex++);

        public RelayCommand GoPrevious => new(() => CurrentIndex--);




        public HomeViewModel(Page page)
        {
            Source = page;
            CurrentStock = Stocks[CurrentIndex];

            Task.Run(LoadPrices);
        }
    }


    public record ValueChange(string Change, SolidColorBrush Color);
}
