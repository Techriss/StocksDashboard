using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FreakinStocksUI.ViewModels
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        public object Source { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
