using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using FreakinStocksUI.ViewModels;
using MaterialDesignThemes.Wpf;

namespace FreakinStocksUI.Models
{
    public class Theme : INotifyPropertyChanged
    {
        private SolidColorBrush _background;
        private SolidColorBrush _middleground;
        private SolidColorBrush _foreground;
        private SolidColorBrush _sideColorAcrylic;
        private SolidColorBrush _sideColor;
        private bool _enableAcrylic = true;
        private ThemeMode _mode;

        public SolidColorBrush Background
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush Middleground
        {
            get => _middleground;
            set
            {
                _middleground = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush Foreground
        {
            get => _foreground;
            set
            {
                _foreground = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush SideColorAcrylic
        {
            get => _sideColorAcrylic;
            set
            {
                _sideColorAcrylic = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush SideColor
        {
            get => _sideColor;
            set
            {
                _sideColor = value;
                OnPropertyChanged();
            }
        }

        public SolidColorBrush Side
        {
            get => Properties.Settings.Default.EnableAcrylic ? EnableAcrylic ? SideColorAcrylic : SideColor : SideColor;
        }

        public bool EnableAcrylic
        {
            get => _enableAcrylic;
            set
            {
                _enableAcrylic = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Side));
            }
        }

        public ThemeMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                OnPropertyChanged();
            }
        }


        public void SetTheme(ThemeMode theme)
        {
            switch (theme)
            {
                case ThemeMode.Light:
                {
                    Background = new(Color.FromRgb(250, 250, 250));
                    Middleground = new(Color.FromRgb(230, 230, 230));
                    Foreground = new(Color.FromRgb(0, 0, 0));
                    SideColorAcrylic = new(Color.FromArgb(221, 230, 230, 230));
                    SideColor = new(Color.FromRgb(230, 230, 230));

                    SetPagesTheme(theme);

                    break;
                }
                case ThemeMode.Dark:
                {
                    Background = new(Color.FromRgb(9, 9, 9));
                    Middleground = new(Color.FromRgb(15, 15, 15));
                    Foreground = new(Color.FromRgb(255, 255, 255));
                    SideColorAcrylic = new(Color.FromArgb(221, 15, 15, 15));
                    SideColor = new(Color.FromRgb(15, 15, 15));

                    SetPagesTheme(theme);

                    break;
                }
            }

            Mode = theme;
            OnPropertyChanged(nameof(Side));
        }

        private void SetPagesTheme(ThemeMode mode)
        {
            var baseTheme = mode.GetBaseTheme();

            ThemeAssist.SetTheme(Application.Current.MainWindow, baseTheme);
            ThemeAssist.SetTheme(MainViewModel.HomePage.Source as DependencyObject, baseTheme);
            ThemeAssist.SetTheme(MainViewModel.AnalyticsPage.Source as DependencyObject, baseTheme);
            ThemeAssist.SetTheme(MainViewModel.LivePage.Source as DependencyObject, baseTheme);
            ThemeAssist.SetTheme(MainViewModel.SearchPage.Source as DependencyObject, baseTheme);
            ThemeAssist.SetTheme(MainViewModel.LikedPage.Source as DependencyObject, baseTheme);
            ThemeAssist.SetTheme(MainViewModel.SettingsPage.Source as DependencyObject, baseTheme);
        }

        public Theme(ThemeMode theme)
        {
            SetTheme(theme);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
