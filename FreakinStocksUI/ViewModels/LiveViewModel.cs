using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using StocksData;

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
        public decimal CurrentPrice => Prices.Any() ? Prices.Last() : 0;

        public ObservableCollection<string> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
                OnPropertyChanged();
            }
        }
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

        public CancellationTokenSource OperationsCancellation { get; set; } = new();

        #endregion



        #region methods

        private async Task FetchLiveData(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                if (DateTime.Now.TimeOfDay.TotalMinutes >= 930 && DateTime.Now.TimeOfDay.TotalMinutes <= 1320 && CurrentStock is not null and not "")
                {
                    var price = (await MainViewModel.Database.LoadAllPricesAsync())?.Where(x => x?.Symbol == CurrentStock).Last();
                    Prices.Add(price.Price);
                    Dates.Add($"{DateTime.Parse(price.Time):t}");
                    OnPropertyChanged(nameof(CurrentPrice));
                }

                await Task.Delay(60000);
            }
        }

        public async Task GetCurrentLiveData()
        {
            var data = (await MainViewModel.Database.LoadAllPricesAsync())?.Where(x => x?.Symbol == CurrentStock && AreDatesEqual(DateTime.Parse(x?.Time), DateTime.Now));

            if (!data.Any())
            {
                var dt = DateTime.Now;

                while (!data.Any() && !IsDateMoreThanWeekAgo(dt))
                {
                    dt = dt.AddDays(-1);
                    data = (await MainViewModel.Database.LoadAllPricesAsync())?.Where(x => x?.Symbol == CurrentStock && AreDatesEqual(DateTime.Parse(x?.Time), dt));
                }
            }

            Prices.Clear();
            Prices.AddRange(data?.Select(x => x?.Price ?? 0));
            Dates = new(data?.Select(x => $"{DateTime.Parse(x.Time):t}"));

            if (!Prices.Any())
            {
                Prices.Add(Convert.ToDecimal(await StockMarketData.GetStockPrice(CurrentStock)));
            }

            OnPropertyChanged(nameof(CurrentPrice));
        }

        private static bool AreDatesEqual(DateTime d1, DateTime d2)
        {
            return d1.Day == d2.Day &&
                   d1.Month == d2.Month &&
                   d1.Year == d2.Year;
        }

        private static bool IsDateMoreThanWeekAgo(DateTime dt)
        {
            return DateTime.Now > dt.AddDays(7);
        }

        #endregion


        public LiveViewModel(Page page)
        {
            Source = page;
            if (Properties.Settings.Default.LiveStock is not "" and not null) CurrentStock = Properties.Settings.Default.LiveStock;

            Task.Run(() => FetchLiveData(OperationsCancellation.Token));
        }
    }
}