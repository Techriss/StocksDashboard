using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class LikedPage : Page
    {
        public LikedPage()
        {
            InitializeComponent();

            this.DataContext = new LikedViewModel(this);
        }
    }
}
