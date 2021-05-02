using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FreakinStocksUI.Models;
using FreakinStocksUI.Views;

namespace FreakinStocksUI.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        #region private

        private Theme _theme = new(ThemeMode.Dark);
        private WindowState _mainWindowState = WindowState.Normal;
        private Page _currentPage;

        #endregion




        #region public

        public Theme AppTheme
        {
            get => _theme;
            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        public WindowState MainWindowState
        {
            get => _mainWindowState;
            set
            {
                _mainWindowState = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MainWindowMargin));
                OnPropertyChanged(nameof(SideColor));
            }
        }

        public Thickness MainWindowMargin => MainWindowState == WindowState.Maximized ? new(0) : new(10);

        public SolidColorBrush SideColor => MainWindowState == WindowState.Maximized ? AppTheme.Side : AppTheme.SideAcrylic;

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

            Debug.WriteLine("Pressed");
        }


        public MainViewModel()
        {
            CurrentPage = HomePage.Source as Page;
        }
    }
}

