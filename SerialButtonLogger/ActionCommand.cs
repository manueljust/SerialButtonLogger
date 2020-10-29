using System;
using System.Windows.Input;

namespace SerialButtonLogger
{
    public class ActionCommand : ICommand
    {
        private Action _action;

        public event EventHandler CanExecuteChanged;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action.Invoke();
        }
    }
}
