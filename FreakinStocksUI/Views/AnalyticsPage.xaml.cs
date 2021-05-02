using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class AnalyticsPage : Page
    {
        public AnalyticsPage()
        {
            InitializeComponent();

            this.DataContext = new AnalyticsViewModel(this);
        }
    }
}
