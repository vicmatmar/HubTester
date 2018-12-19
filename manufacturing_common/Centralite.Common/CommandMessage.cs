using System;
using System.Windows.Input;

namespace Centralite.Common
{
    public class CommandMessage<T> : ICommand
    {
        Action<T> targetExecuteMethod;
        Func<T, bool> targetCanExecuteMethod;

        public CommandMessage(Action<T> executeMethod)
        {
            targetExecuteMethod = executeMethod;
        }

        public CommandMessage(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : this(executeMethod)
        {
            targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged = delegate { };

        bool ICommand.CanExecute(object parameter)
        {
            if (targetCanExecuteMethod != null)
            {
                T tparam = (T)parameter;
                return targetCanExecuteMethod(tparam);
            }
            else
            {
                return (targetExecuteMethod != null);
            }
        }

        void ICommand.Execute(object parameter)
        {
            targetExecuteMethod?.Invoke((T)parameter);
        }
    }

    public class CommandMessage : ICommand
    {
        Action targetExecuteMethod;
        Func<bool> targetCanExecuteMethod;

        public CommandMessage(Action executeMethod)
        {
            targetExecuteMethod = executeMethod;
        }

        public CommandMessage(Action executeMethod, Func<bool> canExecuteMethod) : this(executeMethod)
        {
            targetCanExecuteMethod = canExecuteMethod;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged = delegate { };

        bool ICommand.CanExecute(object parameter)
        {
            if (targetCanExecuteMethod != null)
            {
                return targetCanExecuteMethod();
            }
            else
            {
                return (targetExecuteMethod != null);
            }
        }

        void ICommand.Execute(object parameter)
        {
            targetExecuteMethod?.Invoke();
        }
    }
}
