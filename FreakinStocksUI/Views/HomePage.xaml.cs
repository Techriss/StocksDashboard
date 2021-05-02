using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();

            this.DataContext = new HomeViewModel(this);
        }
    }
}
