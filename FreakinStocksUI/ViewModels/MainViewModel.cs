using System;
using System.Diagnostics;
using System.IO;
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

        public static IDataAccess Database { get; private set; } = Enum.Parse<DatabaseType>(Properties.Settings.Default.DatabaseType) switch
        {
            DatabaseType.MySQL => new MySQLDataAccess(Properties.Settings.Default.DBServer,
                                                      Properties.Settings.Default.DBDatabase,
                                                      Properties.Settings.Default.DBUsername,
                                                      Properties.Settings.Default.DBPasswordCipher,
                                                      Properties.Settings.Default.DBPasswordEntropy),
            DatabaseType.SQLite => new SQLiteDataAccess(),
        };

        public WindowState MainWindowState
        {
            get => _mainWindowState;
            set
            {
                _mainWindowState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MainWindowMargin));
                ThemeAssist.AppTheme.EnableAcrylic = value != WindowState.Maximized;
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

        public HomeViewModel HomePage { get; private set; } = new(new HomePage());

        public AnalyticsViewModel AnalyticsPage { get; private set; } = new(new AnalyticsPage());

        public LiveViewModel LivePage { get; private set; } = new(new LivePage());

        public SearchViewModel SearchPage { get; private set; } = new(new SearchPage());

        public LikedViewModel LikedPage { get; private set; } = new(new LikedPage());

        public SettingsViewModel SettingsPage { get; private set; } = new(new SettingsPage());


        public static bool IsStartupHome => SettingsViewModel.IsStartupHome;
        public static bool IsStartupAnalytics => SettingsViewModel.IsStartupAnalytics;
        public static bool IsStartupLive => SettingsViewModel.IsStartupLive;
        public static bool IsStartupSearch => SettingsViewModel.IsStartupSearch;
        public static bool IsStartupLiked => SettingsViewModel.IsStartupLiked;

        #endregion




        #region commands

        public RelayCommand CloseCommand => new(() => Application.Current.Shutdown());

        public RelayCommand MaximizeCommand => new(() => MainWindowState = MainWindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);

        public RelayCommand MinimizeCommand => new(() => MainWindowState = WindowState.Minimized);

        public RelayCommand NavigateCommand => new((object page) => NavigateTo(Enum.Parse<AppPage>(page as string)));


        #endregion


        public void NavigateTo(AppPage Page)
        {
            switch (Page)
            {
                case AppPage.Home:
                    CurrentPage = HomePage.Source as Page;
                    break;
                case AppPage.Analytics:
                    CurrentPage = AnalyticsPage.Source as Page;
                    break;
                case AppPage.Live:
                    CurrentPage = LivePage.Source as Page;
                    break;
                case AppPage.Search:
                    CurrentPage = SearchPage.Source as Page;
                    break;
                case AppPage.Liked:
                    CurrentPage = LikedPage.Source as Page;
                    break;
                case AppPage.Settings:
                    CurrentPage = SettingsPage.Source as Page;
                    break;
                default:
                    break;
            }
        }

        public void RunLiveService()
        {
            if (Process.GetProcessesByName("FreakinStocksLiveService").Length is 0)
            {
                var psi = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "./FreakinStocksLiveService.exe",
                    UseShellExecute = false,
                };
                var process = new Process() { StartInfo = psi };
                process.Start();

                Properties.Settings.Default.LiveServiceProcessID = process.Id;
                Properties.Settings.Default.Save();
            }
        }

        public static void InstallLiveService()
        {
            try
            {
                var path = Path.GetFullPath(@".\FreakinStocksLiveService.exe");
                var psi = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\system32\sc.exe",
                    Arguments = $"create FreakinStocksLiveData binPath={ path } start= auto",
                    Verb = "runas",
                    UseShellExecute = true,

                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception has occurred: { ex.Message }");
                throw;
            }
        }

        public void KillPreviousLiveService()
        {
            try
            {
                var process = Process.GetProcessById(Properties.Settings.Default.LiveServiceProcessID);

                if (process is not null && process.Id is not 0)
                {
                    process.Kill();
                }
            }
            catch
            {
                Debug.WriteLine("[WARNING] Process cannot be closed");
            }
        }

        public MainViewModel()
        {
            NavigateTo(Enum.Parse<AppPage>(Properties.Settings.Default.StartupPage));

            RunLiveService();

            //InstallLiveService();
        }
    }
}