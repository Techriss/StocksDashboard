using System.Windows;
using System.Windows.Controls;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class Dialog : Window
    {
        public Dialog()
        {
            InitializeComponent();

            this.DataContext = new DialogViewModel() { Source = this };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((dynamic)this.DataContext).Password = ((PasswordBox)sender).SecurePassword;
            }
        }
    }
}