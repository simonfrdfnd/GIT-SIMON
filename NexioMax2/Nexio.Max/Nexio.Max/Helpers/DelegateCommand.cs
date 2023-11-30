using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NexioMax.Helpers
{
  internal class DelegateCommand : ICommand
  {
    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;

    public DelegateCommand(Action<object> execute,
        Predicate<object> canExecute = null)
    {
      _execute = execute;
      _canExecute = canExecute;
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    #region ICommand Members

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    #endregion
  }
}



    