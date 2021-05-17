using System;
using System.Windows;
using System.Windows.Controls;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;
using FreakinStocksUI.Views;
using StocksData;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region private

        private WindowState _mainWindowState = WindowState.Normal;
        private Page _currentPage;

        #endregion



        #region public

        internal static IDataAccess Database { get; private set; } = GetDatabase();

        public WindowState MainWindowState
        {
            get => _mainWindowState;
            set
            {
                _mainWindowState = value;
                ThemeAssist.AppTheme.EnableAcrylic = value is not WindowState.Maximized;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MainWindowMargin));
            }
        }

        public Thickness MainWindowMargin => MainWindowState == WindowState.Maximized ? new(0) : new(10);

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public static HomeViewModel HomePage { get; private set; } = new(new HomePage());
        public static AnalyticsViewModel AnalyticsPage { get; private set; } = new(new AnalyticsPage());
        public static LiveViewModel LivePage { get; private set; } = new(new LivePage());
        public static SearchViewModel SearchPage { get; private set; } = new(new SearchPage());
        public static LikedViewModel LikedPage { get; private set; } = new(new LikedPage());
        public static SettingsViewModel SettingsPage { get; private set; } = new(new SettingsPage());


        public static bool IsStartupHome => SettingsViewModel.IsStartupHome;
        public static bool IsStartupAnalytics => SettingsViewModel.IsStartupAnalytics;
        public static bool IsStartupLive => SettingsViewModel.IsStartupLive;
        public static bool IsStartupSearch => SettingsViewModel.IsStartupSearch;
        public static bool IsStartupLiked => SettingsViewModel.IsStartupLiked;

        #endregion



        #region commands

        public static RelayCommand CloseCommand => new(() => Application.Current.Shutdown());
        public RelayCommand MaximizeCommand => new(() => MainWindowState = MainWindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
        public RelayCommand MinimizeCommand => new(() => MainWindowState = WindowState.Minimized);
        public RelayCommand NavigateCommand => new((object page) => NavigateTo(Enum.Parse<AppPage>(page as string ?? "Home")));

        #endregion



        #region methods

        public void NavigateTo(AppPage Page)
        {
            CurrentPage = Page switch
            {
                AppPage.Home => HomePage.Source as Page,
                AppPage.Analytics => AnalyticsPage.Source as Page,
                AppPage.Live => LivePage.Source as Page,
                AppPage.Search => SearchPage.Source as Page,
                AppPage.Liked => LikedPage.Source as Page,
                AppPage.Settings => SettingsPage.Source as Page,
                _ => CurrentPage ?? HomePage.Source as Page,
            };
        }

        private static IDataAccess GetDatabase(DatabaseType? type = null)
        {
            Action<Exception> handler = (Exception ex) => MessageBox.Show($"An Error has occurred while reading data from the database. Details: { ex.Message }");

            return type switch
            {
                DatabaseType.SQLite => new SQLiteDataAccess(handler),
                DatabaseType.MySQL => new MySQLDataAccess(new(Properties.Settings.Default.DBServer,
                                                              Properties.Settings.Default.DBDatabase,
                                                              Properties.Settings.Default.DBUsername,
                                                              Properties.Settings.Default.DBPasswordEntropy,
                                                              Properties.Settings.Default.DBPasswordCipher),
                                                            handler),
                null => GetDatabase(Enum.Parse<DatabaseType>(Properties.Settings.Default.DatabaseType ?? "SQLite")),
                _ => new SQLiteDataAccess(handler)
            };
        }

        internal static void SetDatabase(DatabaseType? type = null) => Database = GetDatabase(type);

        private static void CheckForInternet()
        {
            if (!StockMarketData.CheckInternetConnection())
            {
                _ = MessageBox.Show("There is no internet connection. The application may not function properly.", "No Internet Connection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion



        public MainViewModel()
        {
            NavigateTo(Enum.Parse<AppPage>(Properties.Settings.Default.StartupPage));
            CheckForInternet();
            ServiceHelper.SetServiceSymbols();
            ServiceHelper.ConfigureLiveService();
        }
    }
}