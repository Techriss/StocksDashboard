using System.ComponentModel;
using System.Runtime.CompilerServices;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public static Theme AppTheme => ThemeAssist.AppTheme;

        public object Source { get; set; }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}