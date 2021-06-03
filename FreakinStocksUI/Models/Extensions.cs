using MaterialDesignThemes.Wpf;
using StocksData.Models;

namespace FreakinStocksUI.Models
{
    /// <summary>
    /// Extensions class for retrieving names of enums and conversion between different enums 
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a <see cref="BaseTheme"/> from the Application <see cref="ThemeMode"/>
        /// </summary>
        /// <param name="mode"><see cref="ThemeMode"/> to be converted to <see cref="BaseTheme"/></param>
        /// <returns><see cref="BaseTheme"/> from the provided <see cref="ThemeMode"/></returns>
        public static BaseTheme GetBaseTheme(this ThemeMode mode)
        {
            return mode switch
            {
                ThemeMode.Light => BaseTheme.Light,
                ThemeMode.Dark => BaseTheme.Dark,
                _ => BaseTheme.Light
            };
        }

        /// <summary>
        /// Replacement for the ToString() method for <see cref="ThemeMode"/> to avoid boxing
        /// </summary>
        /// <param name="mode">The <see cref="ThemeMode"/> which value will be converted to <see cref="string"/></param>
        /// <returns>The <see cref="string"/> from the provided <see cref="ThemeMode"/></returns>
        public static string GetString(this ThemeMode mode)
        {
            return mode switch
            {
                ThemeMode.Light => "Light",
                ThemeMode.Dark => "Dark",
                _ => "Light",
            };
        }


        /// <summary>
        /// Replacement for the ToString() method for <see cref="DatabaseType"/> to avoid boxing
        /// </summary>
        /// <param name="mode">The <see cref="DatabaseType"/> which value will be converted to <see cref="string"/></param>
        /// <returns>The <see cref="string"/> from the provided <see cref="DatabaseType"/></returns>
        public static string GetString(this DatabaseType type)
        {
            return type switch
            {
                DatabaseType.MySQL => "MySQL",
                DatabaseType.SQLite => "SQLite",
                _ => "SQLite",
            };
        }


        /// <summary>
        /// Replacement for the ToString() method for <see cref="AppPage"/> to avoid boxing
        /// </summary>
        /// <param name="mode">The <see cref="AppPage"/> which value will be converted to <see cref="string"/></param>
        /// <returns>The <see cref="string"/> from the provided <see cref="AppPage"/></returns>
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


        /// <summary>
        /// Replacement for the ToString() method for <see cref="DataMode"/> to avoid boxing
        /// </summary>
        /// <param name="mode">The <see cref="DataMode"/> which value will be converted to <see cref="string"/></param>
        /// <returns>The <see cref="string"/> from the provided <see cref="DataMode"/></returns>
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

        /// <summary>
        /// Replacement for the ToString() method for <see cref="HomeStockMode"/> to avoid boxing
        /// </summary>
        /// <param name="mode">The <see cref="HomeStockMode"/> which value will be converted to <see cref="string"/></param>
        /// <returns>The <see cref="string"/> from the provided <see cref="HomeStockMode"/></returns>
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
