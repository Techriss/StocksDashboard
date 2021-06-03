using System;
using System.Windows.Input;

namespace FreakinStocksUI.Models
{
    /// <summary>
    /// The implementation of <see cref="ICommand"/> interface allowing to pass the action which to be executed and its arguments
    /// </summary>
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter = null)
        {
            ToExecute?.Invoke();
            ToExecuteParams?.Invoke(parameter);
        }


        /// <summary>
        /// Constructs a command which executes the provided action with no parameters
        /// </summary>
        /// <param name="toExecute">The action to be executed</param>
        public RelayCommand(Action toExecute)
        {
            ToExecute = toExecute;
        }

        /// <summary>
        /// Constructs a command which executes the provided action with the provided parameters
        /// </summary>
        /// <param name="toExecute">The action with its parameters to be executed</param>
        public RelayCommand(Action<object> toExecute)
        {
            ToExecuteParams = toExecute;
        }



        /// <summary>
        /// The <see cref="Action"/> to be executed when the command is invoked
        /// </summary>
        public Action ToExecute { get; set; } = null;

        /// <summary>
        /// The <see cref="Action{T}"/> to be executed with its parameters when the command is invoked and the parameters may be provided
        /// </summary>
        public Action<object> ToExecuteParams { get; set; } = null;
    }
}
