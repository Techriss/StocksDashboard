using System;
using System.Windows.Controls;
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

        public bool EnableAcrylic
        {
            get => Properties.Settings.Default.EnableAcrylic;
            set
            {
                Properties.Settings.Default.EnableAcrylic = value;
                Properties.Settings.Default.Save();
                ThemeAssist.AppTheme.EnableAcrylic = value;
            }
        }





        public SettingsViewModel(Page page)
        {
            Source = page;
        }
    }
}