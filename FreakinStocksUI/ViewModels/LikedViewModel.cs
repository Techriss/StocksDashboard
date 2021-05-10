using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    class LikedViewModel : ViewModelBase
    {
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

        public RelayCommand RemoveCommand => new((object stock) =>
        {
            Properties.Settings.Default.LikedStocks.Remove((stock as StockEntry).Symbol);
            Properties.Settings.Default.Save();
            Stocks.Remove(stock as StockEntry);
            ReloadCommand.Execute();
            ServiceHelper.SetServiceSymbols();
        });

        public RelayCommand ReloadCommand => new(() => OnPropertyChanged(nameof(Stocks)));

        public LikedViewModel(Page page)
        {
            Source = page;
        }
    }
}