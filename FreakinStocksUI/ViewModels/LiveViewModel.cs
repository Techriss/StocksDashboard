using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using StocksData;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    class LiveViewModel : ViewModelBase
    {
        #region private

        private string _currentStock;
        private ObservableCollection<string> _dates = new();
        private ChartValues<decimal> _prices = new();

        #endregion



        #region public

        /// <summary>
        /// The currently selected stock symbol of a company used by other elements
        /// </summary>
        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                value = value.ToUpper();
                if (StockMarketData.CheckSymbolExists(value))
                {
                    _currentStock = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TempHeaderVisibility));
                    OnPropertyChanged(nameof(DataVisibility));
                    Properties.Settings.Default.LiveStock = value;
                    Properties.Settings.Default.RecentStock = value;
                    Properties.Settings.Default.Save();
                    Task.Run(GetCurrentLiveData);
                }
            }
        }

        /// <summary>
        /// The current real-time updated price for the selected company
        /// </summary>
        public decimal CurrentPrice => Prices.Any() ? Prices.Last() : 0;


        /// <summary>
        /// A collection of points in time for a chart
        /// </summary>
        public ObservableCollection<string> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A list of stock prices for the selected company updated every minute
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


        public Visibility TempHeaderVisibility => CurrentStock is null or "" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility DataVisibility => CurrentStock is null or "" ? Visibility.Collapsed : Visibility.Visible;


        /// <summary>
        /// Enables to cancel the loop of fetching data from the database every minute
        /// </summary>
        public CancellationTokenSource OperationsCancellation { get; set; } = new();

        #endregion



        #region methods

        /// <summary>
        /// Gets real-time data from the database and updates the <see cref="Prices"/> with a new one every minute. Checks the last database entry and adds its value to the chart if its time is different.
        /// </summary>
        /// <param name="token">Cancellation token for cancelling the always-executing data fetching loop</param>
        /// <returns></returns>
        private async Task FetchLiveData(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                if (CurrentStock is not null and not "")
                {
                    var price = (await MainViewModel.Database.LoadAllPricesAsync())?.Where(x => x?.Symbol == CurrentStock).Last();
                    var time = $"{DateTime.Parse(price.Time):t}";

                    if (Dates.Any() && time != Dates.Last())
                    {
                        Prices.Add(price.Price);
                        Dates.Add(time);
                        OnPropertyChanged(nameof(CurrentPrice));
                    }
                }

                await Task.Delay(60000);
            }
        }

        /// <summary>
        /// Gets the currently saved stock prices for the selected stock symbol and sets the <see cref="Prices"/> and <see cref="Dates"/>
        /// </summary>
        /// <returns></returns>
        public async Task GetCurrentLiveData()
        {
            List<StockPrice> data = new();
            var dt = DateTime.Now;

            do
            {
                data = (await MainViewModel.Database.LoadAllPricesAsync())?.Where(x => x?.Symbol == CurrentStock && AreDatesEqual(DateTime.Parse(x?.Time), dt)).ToList();
                dt = dt.AddDays(-1);
            }
            while (!data.Any() && !IsDateMoreThanWeekAgo(dt.AddDays(1)));

            Prices.Clear();
            Prices.AddRange(data?.Select(x => x?.Price ?? 0));
            Dates = new(data?.Select(x => $"{DateTime.Parse(x.Time):t}"));

            if (!Prices.Any())
            {
                Prices.Add(Convert.ToDecimal(await StockMarketData.GetStockPrice(CurrentStock)));
            }

            OnPropertyChanged(nameof(CurrentPrice));
        }

        /// <summary>
        /// Checks if the day, month and year of two provided dates are equal
        /// </summary>
        /// <param name="d1">The first date</param>
        /// <param name="d2">The second date</param>
        /// <returns><see langword="true"/> if the day, month and year of the dates are the same, otherwise <see langword="false"/></returns>
        private static bool AreDatesEqual(DateTime d1, DateTime d2)
        {
            return d1.Day == d2.Day &&
                   d1.Month == d2.Month &&
                   d1.Year == d2.Year;
        }

        /// <summary>
        /// Checks if the provided date was earlier than a week ago
        /// </summary>
        /// <param name="dt">The date to check</param>
        /// <returns><see langword="false"/> if the provided date was later than a week ago, otherwise <see langword="true"/></returns>
        private static bool IsDateMoreThanWeekAgo(DateTime dt)
        {
            return DateTime.Now > dt.AddDays(7);
        }

        #endregion


        /// <summary>
        /// Creates an instance of interaction logic for a Live Data Page and starts fetching data from the database
        /// </summary>
        /// <param name="page">The Live Data Page view</param>
        public LiveViewModel(Page page)
        {
            Source = page;
            if (Properties.Settings.Default.LiveStock is not "" and not null) CurrentStock = Properties.Settings.Default.LiveStock;

            Task.Run(() => FetchLiveData(OperationsCancellation.Token));
        }
    }
}