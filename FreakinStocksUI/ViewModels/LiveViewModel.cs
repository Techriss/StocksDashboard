using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveCharts;
using StocksData;

namespace FreakinStocksUI.ViewModels
{
    class LiveViewModel : ViewModelBase
    {
        private string _currentStock;
        private ObservableCollection<string> _dates;
        private ChartValues<decimal> _prices = new();

        public string CurrentStock
        {
            get
            {
                return _currentStock;
            }
            init
            {
                value = value.ToUpper();
                if (StockMarketData.CheckSymbolExists(value))
                {
                    _currentStock = value;
                    OnPropertyChanged();
                    GetCurrentLiveData();
                }
            }
        }

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




        private async Task FetchLiveData()
        {
            while (true)
            {
                if (DateTime.Now.TimeOfDay.TotalMinutes >= 930 && DateTime.Now.TimeOfDay.TotalMinutes <= 1320 && CurrentStock is not null && CurrentStock != "")
                {
                    var price = (await MainViewModel.StockMarket.LoadAllPricesAsync()).Where(x => x.Symbol == CurrentStock).Last();
                    Prices.Add(price.Price);
                    Dates.Add($"{DateTime.Parse(price.Time):t}");
                }

                await Task.Delay(60000);
            }
        }

        private async Task GetCurrentLiveData()
        {
            var data = (await MainViewModel.StockMarket.LoadAllPricesAsync()).Where(x => x.Symbol == CurrentStock);
            Prices.Clear();
            Prices.AddRange(data.Select(x => x.Price));
            Dates = new(data.Select(x => $"{DateTime.Parse(x.Time):t}"));
        }


        public LiveViewModel(Page page)
        {
            Source = page;

            FetchLiveData();
        }
    }
}