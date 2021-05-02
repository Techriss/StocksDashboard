using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();

            this.DataContext = new SearchViewModel(this);
        }
    }
}
