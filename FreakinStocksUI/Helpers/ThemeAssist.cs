using System;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.Helpers
{
    public static class ThemeAssist
    {
        public static Theme AppTheme { get; set; } = new(Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme));
    }
}
