using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Windows.Controls;
using FreakinStocksUI.Helpers;
using FreakinStocksUI.Models;
using FreakinStocksUI.Views;
using StocksData.Models;

namespace FreakinStocksUI.ViewModels
{
    class SettingsViewModel : ViewModelBase
    {
        public static bool DarkTheme
        {
            get => Enum.Parse<ThemeMode>(Properties.Settings.Default.Theme) == ThemeMode.Dark;
            set
            {
                Properties.Settings.Default.Theme = value ? ThemeMode.Dark.ToString() : ThemeMode.Light.ToString();
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
                    Properties.Settings.Default.DatabaseType = value.ToString();
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
                    Properties.Settings.Default.StartupPage = value.ToString();
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
                    Properties.Settings.Default.AnalyticsStartupPage = value.ToString();
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
                    Properties.Settings.Default.HomeStockMode = value.ToString();
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

        public RelayCommand ChangeDatabaseType => new((object type) => DBType = Enum.Parse<DatabaseType>(type as string));
        public RelayCommand ChangeStartupPage => new((object page) => StartupPage = Enum.Parse<AppPage>(page as string));
        public RelayCommand ChangeAnalyticsStartupPage => new((object mode) => AnalyticsStartupPage = Enum.Parse<DataMode>(mode as string));
        public RelayCommand ConfigureDatabase => new(() => new Dialog().ShowDialog());
        public RelayCommand RestartService => new(() =>
        {
            ServiceHelper.RestartService();
            OnPropertyChanged(nameof(ServiceStatus));
        });
        public RelayCommand StopService => new(() =>
        {
            ServiceHelper.StopService();
            OnPropertyChanged(nameof(ServiceStatus));
        });
        public RelayCommand ReinstallService => new(() =>
        {
            ServiceHelper.ReinstallService();
            OnPropertyChanged(nameof(ServiceStatus));
        });
        public RelayCommand ChangeHomeStock => new((object mode) => HomeStock = Enum.Parse<HomeStockMode>(mode as string));
        public RelayCommand LoadServiceStatus => new(() => OnPropertyChanged(nameof(ServiceStatus)));


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



        public SettingsViewModel(Page page)
        {
            Source = page;
        }
    }
}