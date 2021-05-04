using System;
using System.Windows.Controls;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    class AnalyticsViewModel : ViewModelBase
    {
        private DataMode _currentDataMode;
        private string _currentStock = "TSLA";

        public DataMode CurrentDataMode
        {
            get => _currentDataMode;
            set
            {
                if (value != _currentDataMode)
                {
                    _currentDataMode = value;
                    OnPropertyChanged();
                }
            }
        }



        public string CurrentStock
        {
            get => _currentStock;
            set
            {
                value = value.ToUpper();
                if (StocksData.StockMarketData.CheckSymbolExists(value))
                {
                    _currentStock = value;
                    OnPropertyChanged();
                }
            }
        }




        public RelayCommand SetDataMode => new((object mode) => CurrentDataMode = Enum.Parse<DataMode>(mode as string));



        public AnalyticsViewModel(Page page)
        {
            Source = page;
        }
    }
}