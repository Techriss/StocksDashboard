using System;
using System.Windows.Controls;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    class AnalyticsViewModel : ViewModelBase
    {
        private DataMode _currentDataMode;



        public DataMode CurrentDataMode
        {
            get => _currentDataMode;
            set
            {
                _currentDataMode = value;
                OnPropertyChanged();
            }
        }



        public RelayCommand SetDataMode => new((object mode) => CurrentDataMode = Enum.Parse<DataMode>(mode as string));

        public AnalyticsViewModel(Page page)
        {
            Source = page;
        }
    }
}
