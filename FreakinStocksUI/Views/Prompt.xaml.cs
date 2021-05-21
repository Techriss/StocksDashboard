using System.Windows;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class Prompt : Window
    {
        public Prompt(string header, string content)
        {
            InitializeComponent();

            this.DataContext = new PromptViewModel(header, content, this);
        }
    }
}
