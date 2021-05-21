using System.Windows;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    class PromptViewModel : ViewModelBase
    {
        public string Header { get; init; }

        public string Content { get; init; }

        public bool Result { get; private set; }


        public RelayCommand CloseCommand => new(() =>
        {
            (Source as Window).DialogResult = false;
            (Source as Window).Close();
        });

        public RelayCommand ConfirmCommand => new(() =>
        {
            (Source as Window).DialogResult = true;
            (Source as Window).Close();
        });



        public PromptViewModel(string header, string content, Window source)
        {
            this.Source = source;
            this.Header = header;
            this.Content = content;
        }
    }
}