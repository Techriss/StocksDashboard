using System;
using System.Windows;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.Helpers
{
    /// <summary>
    /// Helper class allowing to set and modify the theme for the entire application
    /// </summary>
    public static class ThemeAssist
    {
        /// <summary>
        /// The single theme property for all elements of the application
        /// </summary>
        public static Theme AppTheme { get; set; } = new(Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme));


        /// <summary>
        /// Uses the <see cref="MaterialDesignThemes"/> to change the theme of the provided <see cref="DependencyObject"/> from the current value of <see cref="AppTheme"/>
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which theme will be modified</param>
        public static void SetThemeForPage(DependencyObject obj)
        {
            MaterialDesignThemes.Wpf.ThemeAssist.SetTheme(obj, AppTheme.Mode.GetBaseTheme());
        }
    }
}
