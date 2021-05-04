using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        public bool DarkTheme
        {
            get => Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme) == ThemeMode.Dark;
            set
            {
                Properties.Settings.Default.Theme = value ? ThemeMode.Dark.ToString() : ThemeMode.Light.ToString();
                Properties.Settings.Default.Save();
                ThemeAssist.AppTheme.SetTheme(Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme));
            }
        }





        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Properties.Settings.Default.Save();
            base.OnPropertyChanged(propertyName);
        }

        public SettingsViewModel(Page page)
        {
            Source = page;
        }
    }


    public static class Extensions
    {
        public static string GetName(this SolidColorBrush brush)
        {
            return nameof(brush);
        }
    }
}