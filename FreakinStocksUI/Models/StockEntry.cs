using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using FreakinStocksUI.Models;
using StocksData;
using YahooFinanceApi;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Record of Stock Market information for a symbol used on the Liked Page of the application. Allows to update the information and execute actions when the symbol is removed from liked ones. Implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public record StockEntry : INotifyPropertyChanged
    {
        /// <summary>
        /// The symbol of a company which Stock Market information is stored
        /// </summary>
        public string Symbol { get; init; }

        /// <summary>
        /// The Stock Market information for a provided company
        /// </summary>
        public Security Stock { get; private set; }

        /// <summary>
        /// Command executed when the specified stock symbol is removed from liked stock symbols
        /// </summary>
        public RelayCommand RemoveCommand { get; init; }

        /// <summary>
        /// Current percent change of the stock price and the color for its value
        /// </summary>
        public ValueChange PriceChange => new(Stock?.RegularMarketChangePercent ?? 0);

        /// <summary>
        /// Determines if the stock symbol was deleted and should be hidden until the list is refreshed
        /// </summary>
        public Visibility EntryVisibility { get; private set; } = Visibility.Visible;

        /// <summary>
        /// Constructs a Stock Entry record for the Liked Page of the application from a provided symbol
        /// </summary>
        /// <param name="symbol">The symbol of a company</param>
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

        /// <summary>
        /// Updates the data with real-time information from the <see cref="YahooFinanceApi"/>
        /// </summary>
        /// <returns></returns>
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