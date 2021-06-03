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
    /// <summary>
    /// Logic for a Search Page displaying stock information
    /// </summary>
    class SearchViewModel : ViewModelBase
    {
        #region private

        private string _currentStock;
        private Security _stockData;

        #endregion



        #region public

        /// <summary>
        /// The currently selected stock symbol of a company
        /// </summary>
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
                    Properties.Settings.Default.RecentStock = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// All available data for the selected stock symbol from <see cref="YahooFinanceApi"/>
        /// </summary>
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


        /// <summary>
        /// The stock price percent change represented by a color
        /// </summary>
        public ValueChange PriceChange
        {
            get
            {
                try
                {
                    return new(StockData?.RegularMarketChangePercent ?? 0);
                }
                catch
                {
                    return new(0);
                }
            }
        }

        /// <summary>
        /// The 50 day avarage percent change represented by a color
        /// </summary>
        public ValueChange FiftyDayAverageChange
        {
            get
            {
                try
                {
                    return new(StockData?.FiftyDayAverageChangePercent ?? 0);
                }
                catch
                {
                    return new(0);
                }
            }
        }

        /// <summary>
        /// The 200 day avarage percent change represented by a color
        /// </summary>
        public ValueChange TwoHundredDayAverageChange
        {
            get
            {
                try
                {
                    return new(StockData?.TwoHundredDayAverageChangePercent ?? 0);
                }
                catch
                {
                    return new(0);
                }
            }
        }

        #endregion



        #region commands

        /// <summary>
        /// Adds or removes the selected stock symbol from the list of liked stocks in settings
        /// </summary>
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

        /// <summary>
        /// Loads all the stock symbol data and the value of it being liked
        /// </summary>
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

        /// <summary>
        /// Loads all the available data for the selected stock from <see cref="YahooFinanceApi"/>
        /// </summary>
        /// <param name="symbol">The symbol of a company to load data for</param>
        /// <returns></returns>
        public async Task LoadData(string symbol)
        {
            var data = await StockMarketData.GetAllStockData(symbol);
            StockData = data;
            OnPropertyChanged(nameof(DataVisibility));
            OnPropertyChanged(nameof(TempHeaderVisibility));
            OnPropertyChanged(nameof(StockData));
        }

        #endregion



        /// <summary>
        /// Creates an instance of interaction logic for a Search Page
        /// </summary>
        /// <param name="page">The Search Page view</param>
        public SearchViewModel(Page page)
        {
            Source = page;
            if (Properties.Settings.Default.SearchStock is not "" and not null) CurrentStock = Properties.Settings.Default.SearchStock;
        }
    }
}