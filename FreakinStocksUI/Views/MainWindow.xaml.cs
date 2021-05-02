using System.Windows;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
        }
    }
}
