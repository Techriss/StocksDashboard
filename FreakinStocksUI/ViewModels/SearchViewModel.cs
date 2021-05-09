using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;
using MaterialDesignThemes.Wpf;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    class SearchViewModel : ViewModelBase
    {
        #region private

        private string _currentStock;
        private Security _stockData;

        #endregion



        #region public

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
                    OnPropertyChanged(nameof(IsCurrentStockLiked));
                    Properties.Settings.Default.SearchStock = value;
                    Properties.Settings.Default.Save();
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
                OnPropertyChanged(nameof(PriceChange));
                OnPropertyChanged(nameof(FiftyDayAverageChange));
                OnPropertyChanged(nameof(TwoHundredDayAverageChange));
            }
        }

        public Visibility TempHeaderVisibility => CurrentStock is null or "" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DataVisibility => CurrentStock is null or "" ? Visibility.Collapsed : Visibility.Visible;
        public PackIconKind IsCurrentStockLiked => Properties.Settings.Default.LikedStocks is not null && Properties.Settings.Default.LikedStocks.Contains(CurrentStock) ? PackIconKind.Heart : PackIconKind.HeartOutline;

        public ValueChange PriceChange
        {
            get
            {
                return new($"{ Math.Round(StockData?.RegularMarketChangePercent ?? 0, 2) }%", ValueChange.GetColorForValue(StockData?.RegularMarketChangePercent ?? 0));
            }
        }
        public ValueChange FiftyDayAverageChange
        {
            get
            {
                return new($"{ Math.Round(StockData?.FiftyDayAverageChangePercent ?? 0, 2) }%", ValueChange.GetColorForValue(StockData?.FiftyDayAverageChangePercent ?? 0));
            }
        }
        public ValueChange TwoHundredDayAverageChange
        {
            get
            {
                return new($"{ Math.Round(StockData?.TwoHundredDayAverageChangePercent ?? 0, 2) }%", ValueChange.GetColorForValue(StockData?.TwoHundredDayAverageChangePercent ?? 0));
            }
        }

        #endregion



        #region commands

        public RelayCommand ChangeLikedCommand => new(() =>
        {
            if (Properties.Settings.Default.LikedStocks is null) Properties.Settings.Default.LikedStocks = new();
            if (!Properties.Settings.Default.LikedStocks.Contains(CurrentStock))
            {
                Properties.Settings.Default.LikedStocks.Add(CurrentStock);
            }
            else
            {
                Properties.Settings.Default.LikedStocks.Remove(CurrentStock);
            }

            Properties.Settings.Default.Save();
            ServiceHelper.SetServiceSymbols();
            OnPropertyChanged(nameof(IsCurrentStockLiked));
        });

        public RelayCommand ReloadCommand => new(async () =>
        {
            if (CurrentStock is not null) await LoadData(CurrentStock);
            OnPropertyChanged(nameof(StockData));
            OnPropertyChanged(nameof(DataVisibility));
            OnPropertyChanged(nameof(TempHeaderVisibility));
            OnPropertyChanged(nameof(IsCurrentStockLiked));
        });

        #endregion



        #region methods

        public async Task LoadData(string symbol)
        {
            var data = await StockMarketData.GetAllStockData(symbol);
            StockData = data;
            OnPropertyChanged(nameof(DataVisibility));
            OnPropertyChanged(nameof(TempHeaderVisibility));
            OnPropertyChanged(nameof(StockData));
        }

        #endregion



        public SearchViewModel(Page page)
        {
            Source = page;
            if (Properties.Settings.Default.SearchStock is not "" or null) CurrentStock = Properties.Settings.Default.SearchStock;
        }
    }
}