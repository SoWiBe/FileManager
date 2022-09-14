using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileManager.ViewModels.Commands
{
    public class MaximizeCommand : ICommand
    {
        private Action execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public MaximizeCommand(Action execute, Func<object, bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentException("exception");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute.Invoke();
        }
    }
}
