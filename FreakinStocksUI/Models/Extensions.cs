using MaterialDesignThemes.Wpf;
using StocksData.Models;

namespace FreakinStocksUI.Models
{
    public static class Extensions
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

        public static string GetString(this ThemeMode mode)
        {
            return mode switch
            {
                ThemeMode.Light => "Light",
                ThemeMode.Dark => "Dark",
                _ => "Light",
            };
        }


        public static string GetString(this DatabaseType type)
        {
            return type switch
            {
                DatabaseType.MySQL => "MySQL",
                DatabaseType.SQLite => "SQLite",
                _ => "SQLite",
            };
        }



        public static string GetString(this AppPage page)
        {
            return page switch
            {
                AppPage.Home => "Home",
                AppPage.Analytics => "Analytics",
                AppPage.Live => "Live",
                AppPage.Search => "Search",
                AppPage.Liked => "Liked",
                AppPage.Settings => "Settings",
                _ => "Home",
            };
        }


        public static string GetString(this DataMode mode)
        {
            return mode switch
            {
                DataMode.Week => "Week",
                DataMode.Month => "Month",
                DataMode.Year => "Year",
                DataMode.All => "All",
                _ => "Year",
            };
        }

        public static string GetString(this HomeStockMode mode)
        {
            return mode switch
            {
                HomeStockMode.Liked => "Liked",
                HomeStockMode.Recent => "Recent",
                _ => "Liked",
            };
        }
    }
}
