namespace FreakinStocksUI.Models
{
    /// <summary>
    /// Valid Application Themes
    /// </summary>
    public enum ThemeMode
    {
        Light,
        Dark
    }

    /// <summary>
    /// Valid Application Pages
    /// </summary>
    public enum AppPage
    {
        Home,
        Analytics,
        Live,
        Search,
        Liked,
        Settings
    }

    /// <summary>
    /// Valid Modes for stock prices data
    /// </summary>
    public enum DataMode
    {
        Week,
        Month,
        Year,
        All
    }

    /// <summary>
    /// Valid Modes of Home Page's stock symbols
    /// </summary>
    public enum HomeStockMode
    {
        Liked,
        Recent
    }
}
