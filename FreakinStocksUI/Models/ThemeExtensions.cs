using MaterialDesignThemes.Wpf;

namespace FreakinStocksUI.Models
{
    public static class ThemeExtensions
    {
        public static BaseTheme GetBaseTheme(this ThemeMode mode)
        {
            return mode switch
            {
                ThemeMode.Light => BaseTheme.Light,
                ThemeMode.Dark => BaseTheme.Dark,
                _ => BaseTheme.Light
            };
        }
    }
}
