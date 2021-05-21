using System;
using System.Windows;
using FreakinStocksUI.ViewModels;

namespace FreakinStocksUI.Views
{
    public partial class Prompt : Window
    {
        public Prompt(string header, string content, bool confirmation = false)
        {
            InitializeComponent();

            this.DataContext = new PromptViewModel(header, content, this, confirmation);
        }

        public Prompt(Exception ex)
        {
            this.DataContext = new PromptViewModel(ex, this);
        }
    }
}
