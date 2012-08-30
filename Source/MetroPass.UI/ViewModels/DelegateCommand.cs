
using System;
using System.Windows.Input;



namespace MetroPass.UI.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private Action _action;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}