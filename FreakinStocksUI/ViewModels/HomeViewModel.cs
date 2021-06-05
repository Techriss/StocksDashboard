using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using FreakinStocksUI.Models;
using LiveCharts;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Logic implementation for the Home Page
    /// </summary>
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

        /// <summary>
        /// The currently selected stock symbol for a company which information will be shown.
        /// Reloads the chart when set.
        /// </summary>
        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                _currentStock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentDisplayStock));

                _ = Task.Run(LoadPrices);
            }
        }

        /// <summary>
        /// The selected stock symbol all uppercase with every character separated by space
        /// </summary>
        /// <example>
        /// If the symbol is 'TSLA', the <see cref="CurrentDisplayStock"/> will be 'T S L A'
        /// </example>
        public string CurrentDisplayStock => CurrentStock is null ? "" : string.Join(" ", CurrentStock?.ToCharArray());


        /// <summary>
        /// List of prices for the selected company from last week used by <see cref="LiveCharts"/>
        /// </summary>
        public ChartValues<decimal> Prices
        {
            get => _prices;
            set
            {
                _prices = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// List of dates from last week
        /// </summary>
        public List<string> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Basic stock information for the specified symbol
        /// </summary>
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

        /// <summary>
        /// The percent of change of the stock price
        /// </summary>
        public ValueChange PriceChange
        {
            get
            {
                return new(StockInfo?.RegularMarketChangePercent ?? 0);
            }
        }


        /// <summary>
        /// An array of stock symbols available to choose from in the home page
        /// </summary>
        public string[] Stocks { get; private set; }

        /// <summary>
        /// The current index in the <see cref="Stocks"/> array representing a symbol
        /// </summary>
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

        /// <summary>
        /// Tells if the current index is the last from <see cref="Stocks"/>
        /// </summary>
        public bool CanGoNext => CurrentIndex + 1 <= Stocks.Length - 1;

        /// <summary>
        /// Tells if the current index is the first from <see cref="Stocks"/>
        /// </summary>
        public bool CanGoPrevious => CurrentIndex - 1 >= 0;


        /// <summary>
        /// Gets an appropriate welcome phrase for the current time of day
        /// </summary>
        public static string WelcomeHeader => DateTime.Now.TimeOfDay.TotalHours switch
        {
            >= 5 and < 12 => "Good Morning!",
            >= 12 and < 17 => "Good Afternoon!",
            >= 17 or < 5 => "Good Evening!",
            _ => "Welcome Back!"
        };


        #endregion



        #region commands

        /// <summary>
        /// Increments the current index from <see cref="Stocks"/>. The error handling is done externally.
        /// </summary>
        public RelayCommand GoNext => new(() => CurrentIndex++);

        /// <summary>
        /// Decrements the current index from <see cref="Stocks"/>. The error handling is done externally.
        /// </summary>
        public RelayCommand GoPrevious => new(() => CurrentIndex--);

        /// <summary>
        /// Reloads the last week prices of the selected company
        /// </summary>
        public RelayCommand ReloadCommand => new(async () => await LoadPrices());

        /// <summary>
        /// Overrides the <see cref="ViewModelBase.MoveFocus"/> method to allow changing the current index in <see cref="Stocks"/> using arrow keys
        /// </summary>
        public override RelayCommand MoveFocus => new(() =>
        {
            if (Keyboard.IsKeyDown(Key.Left) && CanGoPrevious)
            {
                GoPrevious.Execute();
            }
            else if (Keyboard.IsKeyDown(Key.Right) && CanGoNext)
            {
                GoNext.Execute();
            }
        });

        #endregion



        #region methods

        /// <summary>
        /// Loads the last week prices for the selected company
        /// </summary>
        /// <returns></returns>
        private async Task LoadPrices()
        {
            Stocks = GetStocks();
            OnPropertyChanged(nameof(CurrentStock));
            OnPropertyChanged(nameof(CurrentDisplayStock));
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(WelcomeHeader));

            if (CurrentIndex > Stocks.Length)
            {
                CurrentIndex = 0;
            }

            try
            {
                var data = (await StockMarketData.GetLastWeek(CurrentStock)).ToList();
                var prices = data?.Select(x => x?.Price ?? 0);
                var dates = data?.Select(x => $"{DateTime.Parse(x.Time):dddd}").ToList();
                var info = await StockMarketData.GetStockData(CurrentStock);

                Prices = new(prices);
                Dates = dates;
                StockInfo = info;
            }
            catch
            {
                Debug.WriteLine("Invalid symbol");
            }
        }

        /// <summary>
        /// Gets an array of stock symbols from settings appropriately to the <see cref="HomeStockMode"/> or gets the defaults
        /// </summary>
        /// <returns>An array of stock symbols retrieved from settings</returns>
        private string[] GetStocks()
        {
            if (Enum.Parse<HomeStockMode>(Properties.Settings.Default.HomeStockMode) is HomeStockMode.Liked && Properties.Settings.Default.LikedStocks?.Count > 0 ||
                Properties.Settings.Default.RecentStock == "")
            {
                return Properties.Settings.Default.LikedStocks?.ToArray() ?? new[] { "TSLA", "NDAQ", "AAPL" };
            }
            else
            {
                _currentStock = Properties.Settings.Default.RecentStock;
                return new[] { Properties.Settings.Default.RecentStock };
            }
        }

        #endregion


        /// <summary>
        /// Creates an instance of the logic for a Home Page
        /// </summary>
        /// <param name="page">The Home Page view</param>
        public HomeViewModel(Page page)
        {
            Source = page;
            Stocks = GetStocks();
            _currentStock = Stocks[CurrentIndex];

            _ = Task.Run(LoadPrices);
        }
    }
}
