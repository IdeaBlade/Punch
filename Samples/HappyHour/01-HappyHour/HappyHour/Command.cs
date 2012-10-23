using System;
using System.Windows.Input;

// Located in Cocktail_101/Mixers

namespace HappyHour
{
    /// <summary>
    /// A generalized <see cref="ICommand"/> implementation
    /// </summary>
    /// <remarks>
    /// This rather standard XAML ICommand class is not needed in Cocktail/Caliburn.Micro apps.
    /// </remarks>
    public class Command : ICommand
    {

        private readonly Action<object> _execute = delegate {};
        private readonly Func<object, bool> _canExecute = delegate {return true;};

        // Constructor for command that takes a parameter
        public Command(
            Action<object> execute,
            Func<object, bool> canExecute = null)
        {
            if (null != execute) _execute = execute;
            if (null != canExecute) _canExecute = canExecute;
        }

        // Constructor for command that doesn't take a parameter
        public Command(
            Action execute,
            Func<object, bool> canExecute = null) 
        {
            if (null != execute) _execute = _ => execute();
            if (null != canExecute) _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var handlers = CanExecuteChanged;
            if (null != handlers)
            {
                handlers(this, EventArgs.Empty);
            }
        }
    }
}