using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public static Theme AppTheme => ThemeAssist.AppTheme;

        public object Source { get; set; }

        public virtual RelayCommand MoveFocus => new(() =>
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                var focused = Keyboard.FocusedElement as UIElement;
                focused?.MoveFocus(new(FocusNavigationDirection.Next));
            }
        });


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}