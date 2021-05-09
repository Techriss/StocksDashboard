using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FreakinStocksUI.Models;
using LiveCharts;
using StocksData;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    class AnalyticsViewModel : ViewModelBase
    {
        #region private

        private DataMode _currentDataMode = Enum.Parse<DataMode>(Properties.Settings.Default.AnalyticsStartupPage ?? "All");
        private string _currentStock;
        private ChartValues<decimal> _prices;
        private List<string> _dates = new();

        #endregion



        #region public

        public DataMode CurrentDataMode
        {
            get => _currentDataMode;
            set
            {
                if (value != _currentDataMode)
                {
                    _currentDataMode = value;
                    OnPropertyChanged();

                    if (CurrentStock is not null or "")
                    {
                        Task.Run(() => LoadChart(CurrentDataMode));
                    }
                }
            }
        }
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
                    OnPropertyChanged(nameof(ChartVisibility));
                    OnPropertyChanged(nameof(TempHeaderVisibility));
                    Properties.Settings.Default.AnalyticsStock = value;
                    Properties.Settings.Default.Save();
                    Task.Run(() => LoadChart(CurrentDataMode));
                }
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
        public List<string> Dates
        {
            get => _dates;
            set
            {
                _dates = value;
                OnPropertyChanged();
            }
        }

        public Visibility TempHeaderVisibility => CurrentStock is null or "" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ChartVisibility => CurrentStock is null or "" ? Visibility.Collapsed : Visibility.Visible;

        public bool IsAll => CurrentDataMode is DataMode.All;
        public bool IsYear => CurrentDataMode is DataMode.Year;
        public bool IsMonth => CurrentDataMode is DataMode.Month;
        public bool IsWeek => CurrentDataMode is DataMode.Week;

        #endregion



        #region command

        public RelayCommand SetDataMode => new((object mode) => CurrentDataMode = Enum.Parse<DataMode>(mode as string));

        #endregion



        #region methods

        public async Task LoadChart(DataMode mode)
        {
            IEnumerable<StockPrice> data = null;

            switch (mode)
            {
                case DataMode.Week:
                {
                    data = await StockMarketData.GetLastWeek(CurrentStock);
                    Dates = data.Select(x => $"{DateTime.Parse(x.Time):dddd}").ToList();
                    Prices = new(data.Select(x => x.Price));
                    break;
                }
                case DataMode.Month:
                {
                    data = await StockMarketData.GetLastMonth(CurrentStock);
                    Dates = data.Select(x => $"{DateTime.Parse(x.Time):M}").ToList();
                    Prices = new(data.Select(x => x.Price)); break;
                }
                case DataMode.Year:
                {
                    data = await StockMarketData.GetLastYear(CurrentStock);
                    Dates = data.Select(x => $"{DateTime.Parse(x.Time):Y}").ToList();
                    Prices = new(data.Select(x => x.Price));
                    break;
                }
                case DataMode.All:
                {
                    data = await StockMarketData.GetAllTime(CurrentStock);
                    Dates = data.Select(x => $"{DateTime.Parse(x.Time):Y}").ToList();
                    Prices = new(data.Select(x => x.Price));
                    break;
                }
            }
        }

        #endregion



        public AnalyticsViewModel(Page page)
        {
            Source = page;
            if (Properties.Settings.Default.AnalyticsStock is not "" or null) CurrentStock = Properties.Settings.Default.AnalyticsStock;
        }
    }
}