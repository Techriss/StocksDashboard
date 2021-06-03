using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// The base of a view model representing interaction logic for an application element.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The currently selected application theme used by XAML controls
        /// </summary>
        public static Theme AppTheme => ThemeAssist.AppTheme;

        /// <summary>
        /// The element's view
        /// </summary>
        public object Source { get; set; }

        /// <summary>
        /// Allows for enter key focus change functionality
        /// </summary>
        public virtual RelayCommand MoveFocus => new(() =>
        {
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                var focused = Keyboard.FocusedElement as UIElement;
                focused?.MoveFocus(new(FocusNavigationDirection.Next));
            }
        });


        /// <summary>
        /// Allows the view element to automatically refresh when the function occurs
        /// </summary>
        /// <param name="propertyName">The property which was changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}