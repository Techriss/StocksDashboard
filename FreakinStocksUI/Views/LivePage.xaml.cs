using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class LivePage : Page
    {
        public LivePage()
        {
            InitializeComponent();

            this.DataContext = new LiveViewModel(this);
        }
    }
}
