
using System;
using System.Threading.Tasks;
using System.Windows.Input;



namespace MetroPass.UI.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private Action<object> _action;
        private Func<object, Task> _asyncAction;

        public DelegateCommand(Action<object> action)
        {
            _action = action;
        }

        public DelegateCommand(Func<object, Task> asyncAction)
        {
            _asyncAction = asyncAction;
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public async void Execute(object parameter)
        {
            if (_action != null)
            {
                _action(parameter);
            }
            else if(_asyncAction !=null)
            {
                await _asyncAction(parameter);
            }
           
          
        }
    }
}