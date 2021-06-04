using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Controls;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;
using FreakinStocksUI.Views;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Logic for a Settings Page
    /// </summary>
    class SettingsViewModel : ViewModelBase
    {
        public static bool DarkTheme
        {
            get => Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme) == ThemeMode.Dark;
            set
            {
                Properties.Settings.Default.Theme = value ? ThemeMode.Dark.GetString() : ThemeMode.Light.GetString();
                Properties.Settings.Default.Save();
                ThemeAssist.AppTheme.SetTheme(Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme));
            }
        }
        public static bool EnableAcrylic
        {
            get => Properties.Settings.Default.EnableAcrylic;
            set
            {
                Properties.Settings.Default.EnableAcrylic = value;
                Properties.Settings.Default.Save();
                ThemeAssist.AppTheme.EnableAcrylic = value;
            }
        }

        public DatabaseType DBType
        {
            get => Enum.Parse<DatabaseType>(Properties.Settings.Default.DatabaseType);
            set
            {
                if (DBType != value && ((value == DatabaseType.MySQL && new Dialog().ShowDialog().Value) || value == DatabaseType.SQLite))
                {
                    Properties.Settings.Default.DatabaseType = value.GetString();
                    Properties.Settings.Default.Save();
                    MainViewModel.SetDatabase(value);
                }
                else
                {
                    RefreshDatabaseChoice();
                }
                if (DBType == DatabaseType.SQLite)
                {
                    try
                    {
                        File.WriteAllLines("MySQL.txt", new[] { "false" });
                    }
                    catch
                    {
                        Debug.WriteLine("[ERR] Cannot save to MySQL.txt file");
                    }
                }
            }
        }
        public static AppPage StartupPage
        {
            get => Enum.Parse<AppPage>(Properties.Settings.Default.StartupPage);
            set
            {
                if (StartupPage != value)
                {
                    Properties.Settings.Default.StartupPage = value.GetString();
                    Properties.Settings.Default.Save();
                }
            }
        }
        public static DataMode AnalyticsStartupPage
        {
            get => Enum.Parse<DataMode>(Properties.Settings.Default.AnalyticsStartupPage);
            set
            {
                if (AnalyticsStartupPage != value)
                {
                    Properties.Settings.Default.AnalyticsStartupPage = value.GetString();
                    Properties.Settings.Default.Save();
                }
            }
        }
        public static HomeStockMode HomeStock
        {
            get => Enum.Parse<HomeStockMode>(Properties.Settings.Default.HomeStockMode);
            set
            {
                if (value != HomeStock)
                {
                    Properties.Settings.Default.HomeStockMode = value.GetString();
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool IsMySQLSelected
        {
            get => DBType is DatabaseType.MySQL;
            set => DBType = value ? DatabaseType.MySQL : DBType;
        }
        public bool IsSQLiteSelected
        {
            get => DBType is DatabaseType.SQLite;
            set => DBType = value ? DatabaseType.SQLite : DBType;
        }

        public static bool IsStartupHome => StartupPage is AppPage.Home;
        public static bool IsStartupAnalytics => StartupPage is AppPage.Analytics;
        public static bool IsStartupLive => StartupPage is AppPage.Live;
        public static bool IsStartupSearch => StartupPage is AppPage.Search;
        public static bool IsStartupLiked => StartupPage is AppPage.Liked;

        public static bool IsAnalyticsStartupWeek => AnalyticsStartupPage is DataMode.Week;
        public static bool IsAnalyticsStartupMonth => AnalyticsStartupPage is DataMode.Month;
        public static bool IsAnalyticsStartupYear => AnalyticsStartupPage is DataMode.Year;
        public static bool IsAnalyticsStartupAll => AnalyticsStartupPage is DataMode.All;

        public static bool IsHomeStockLiked => HomeStock is HomeStockMode.Liked;
        public static bool IsHomeStockRecent => HomeStock is HomeStockMode.Recent;

        public string ServiceStatus => GetServiceStatus();
        public long DatabaseEntriesNumber => MainViewModel.Database.GetEntriesNumber();

        public RelayCommand ChangeDatabaseType => new((object type) => DBType = Enum.Parse<DatabaseType>(type as string));
        public RelayCommand ChangeStartupPage => new((object page) => StartupPage = Enum.Parse<AppPage>(page as string));
        public RelayCommand ChangeAnalyticsStartupPage => new((object mode) => AnalyticsStartupPage = Enum.Parse<DataMode>(mode as string));
        public RelayCommand ConfigureDatabase => new(() => new Dialog().ShowDialog());
        public RelayCommand RestartService => new(async () =>
        {
            ServiceHelper.RestartService();
            await Task.Delay(500);
            OnPropertyChanged(nameof(ServiceStatus));
        });
        public RelayCommand StopService => new(async () =>
        {
            ServiceHelper.StopService();
            await Task.Delay(500);
            OnPropertyChanged(nameof(ServiceStatus));
        });
        public RelayCommand ReinstallService => new(async () =>
        {
            ServiceHelper.ReinstallService();
            await Task.Delay(500);
            OnPropertyChanged(nameof(ServiceStatus));
        });
        public RelayCommand ChangeHomeStock => new((object mode) => HomeStock = Enum.Parse<HomeStockMode>(mode as string));
        public RelayCommand Reload => new(() =>
        {
            OnPropertyChanged(nameof(ServiceStatus));
            OnPropertyChanged(nameof(DatabaseEntriesNumber));
        });
        public static RelayCommand ClearAll => new(async () =>
        {
            if (new Prompt("Confirm Deleting", "Are you sure to clear all saved live data in the selected database?", true).ShowDialog().Value)
            {
                await MainViewModel.Database.ClearDatabaseAsync();
                _ = new Prompt("Action Completed", "The database was cleared successfully.").ShowDialog();
            }
        });
        public static RelayCommand ViewLog => new(ViewServiceLog);
        public static RelayCommand ClearLog => new(ClearServiceLog);


        public void RefreshDatabaseChoice()
        {
            OnPropertyChanged(nameof(IsSQLiteSelected));
            OnPropertyChanged(nameof(IsMySQLSelected));
        }

        public static string GetServiceStatus()
        {
            return ServiceHelper.GetService()?.Status switch
            {
                ServiceControllerStatus.Stopped => "Stopped",
                ServiceControllerStatus.StartPending => "Pending Start",
                ServiceControllerStatus.StopPending => "Pending Stop",
                ServiceControllerStatus.Running => "Running",
                ServiceControllerStatus.ContinuePending => "Pending Continue",
                ServiceControllerStatus.PausePending => "Pending Pause",
                ServiceControllerStatus.Paused => "Paused",
                _ => "Unavailable",
            };
        }

        private static void ViewServiceLog()
        {
            try
            {
                if (File.Exists(FreakinStocksLiveService.Program.LOG))
                {
                    Process.Start("notepad.exe", FreakinStocksLiveService.Program.LOG);
                }
                else
                {
                    _ = new Prompt("Could not find the log", "The service log could not be found. Make sure the service was running.").ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _ = new Prompt(ex).ShowDialog();
            }
        }

        private static void ClearServiceLog()
        {
            try
            {
                if (File.Exists(FreakinStocksLiveService.Program.LOG))
                {
                    File.WriteAllText(FreakinStocksLiveService.Program.LOG, string.Empty);
                    _ = new Prompt("Log cleared", "The service log was cleared successfully.").ShowDialog();
                }
                else
                {
                    _ = new Prompt("Could not find the log", "The service log could not be found. Make sure the service was running.").ShowDialog();
                }
            }
            catch (IOException)
            {
                _ = new Prompt("Log in use", "Could not clear the file. Stop the Live Data Service before clearing the log.").ShowDialog();
            }
            catch
            {
                _ = new Prompt("Clearing Failed", "Could not access the service log.").ShowDialog();
            }
        }



        /// <summary>
        /// Creates an instance of interaction logic for a Settings Page
        /// </summary>
        /// <param name="page">The Settings Page view</param>
        public SettingsViewModel(Page page)
        {
            Source = page;
        }
    }
}