using System;
using System.IO;
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

        public static DatabaseType DBType
        {
            get => Enum.Parse<DatabaseType>(Properties.Settings.Default.DatabaseType);
            set
            {
                if (DBType != value && ((value == DatabaseType.MySQL && new Dialog().ShowDialog().Value) || value == DatabaseType.SQLite))
                {
                    Properties.Settings.Default.DatabaseType = value.ToString();
                    Properties.Settings.Default.Save();
                }
                if (DBType == DatabaseType.SQLite)
                {
                    try
                    {
                        File.WriteAllLines("MySQL.txt", new[] { "false" });
                    }
                    catch { }
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

        public static bool IsMySQLSelected => DBType is DatabaseType.MySQL;
        public static bool IsSQLiteSelected => DBType is DatabaseType.SQLite;

        public static bool IsStartupHome => StartupPage is AppPage.Home;
        public static bool IsStartupAnalytics => StartupPage is AppPage.Analytics;
        public static bool IsStartupLive => StartupPage is AppPage.Live;
        public static bool IsStartupSearch => StartupPage is AppPage.Search;
        public static bool IsStartupLiked => StartupPage is AppPage.Liked;


        public static RelayCommand ChangeDatabaseType => new((object type) => DBType = Enum.Parse<DatabaseType>(type as string));
        public static RelayCommand ChangeStartupPage => new((object page) => StartupPage = Enum.Parse<AppPage>(page as string));
        public static RelayCommand ConfigureDatabase => new(() => new Dialog().ShowDialog());
        public static RelayCommand InstallService => new(() => ServiceHelper.ConfigureLiveService());


        public SettingsViewModel(Page page)
        {
            Source = page;
        }
    }
}