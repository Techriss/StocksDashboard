using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using FreakinStocksUI.Models;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    public record StockEntry : INotifyPropertyChanged
    {
        public string Symbol { get; init; }

        public Security Stock { get; private set; }

        public RelayCommand RemoveCommand { get; init; }

        public ValueChange PriceChange => new(Stock?.RegularMarketChangePercent ?? 0);

        public Visibility EntryVisibility { get; private set; } = Visibility.Visible;

        public StockEntry(string symbol)
        {
            Symbol = symbol;
            Task.Run(GetStockData).ConfigureAwait(false);
            RemoveCommand = new(() =>
            {
                MainViewModel.LikedPage.RemoveCommand.Execute(this);
                EntryVisibility = Visibility.Collapsed;
                OnPropertyChanged(nameof(EntryVisibility));
            });
        }

        private async Task GetStockData()
        {
            Stock = await StockMarketData.GetStockData(Symbol);
            OnPropertyChanged(nameof(Stock));
            OnPropertyChanged(nameof(PriceChange));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}