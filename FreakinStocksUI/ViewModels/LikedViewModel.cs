using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Logic implementation for a Liked Page
    /// </summary>
    class LikedViewModel : ViewModelBase
    {
        /// <summary>
        /// A collection of Stock Price Entries which can be used for list values
        /// </summary>
        public ObservableCollection<StockEntry> Stocks
        {
            get
            {
                var stocks = Properties.Settings.Default.LikedStocks ?? new();
                var stocksentries = new List<StockEntry>();
                stocks.ForEach(s => stocksentries.Add(new(s)));

                return new(stocksentries);
            }
        }

        /// <summary>
        /// Occurs when an element is removed from the list of liked stock symbols
        /// </summary>
        public RelayCommand RemoveCommand => new((object stock) =>
        {
            Properties.Settings.Default.LikedStocks?.Remove((stock as StockEntry).Symbol);
            Properties.Settings.Default.Save();
            Stocks.Remove(stock as StockEntry);
            ReloadCommand.Execute();
            ServiceHelper.SetServiceSymbols();
        });

        /// <summary>
        /// Loads the list of liked stock symbols by invoking the get method for <see cref="Stocks"/>
        /// </summary>
        public RelayCommand ReloadCommand => new(() => OnPropertyChanged(nameof(Stocks)));

        /// <summary>
        /// Creates an instance of interaction logic for a Liked Page
        /// </summary>
        /// <param name="page">The Liked Page view</param>
        public LikedViewModel(Page page)
        {
            Source = page;
        }
    }
}