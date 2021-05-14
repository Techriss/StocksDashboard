using System;
using System.Windows;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.Helpers
{
    public static class ThemeAssist
    {
        public static Theme AppTheme { get; set; } = new(Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme));

        public static void SetThemeForPage(DependencyObject obj)
        {
            MaterialDesignThemes.Wpf.ThemeAssist.SetTheme(obj, AppTheme.Mode.GetBaseTheme());
        }
    }
}
