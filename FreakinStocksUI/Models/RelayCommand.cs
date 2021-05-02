using System;
using System.Windows.Input;

namespace FreakinStocksUI.Models
{
    class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ToExecute?.Invoke();
            ToExecuteParams?.Invoke(parameter);
        }


        public RelayCommand(Action toExecute)
        {
            ToExecute = toExecute;
        }

        public RelayCommand(Action<object> toExecute)
        {
            ToExecuteParams = toExecute;
        }



        public Action ToExecute { get; set; } = null;

        public Action<object> ToExecuteParams { get; set; } = null;
    }
}
