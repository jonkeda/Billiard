using System;
using System.Windows.Input;
using Billiards.Base.Exceptions;
using Billiards.Wpf.Extensions;

namespace Billiards.Wpf.UI
{
    public class TargetCommand<T> : ICommand
    {
        private readonly CommandCanExecuteDelegate<T> _canExecute;
        private readonly CommandDelegate<T> _method;
        private readonly bool _showWaitCursor = true;

        public TargetCommand(CommandDelegate<T> method)
        {
            _method = method;
        }

        public TargetCommand(CommandDelegate<T> method, bool showWaitCursor)
        {
            _method = method;
            _showWaitCursor = showWaitCursor;
        }

        public TargetCommand(CommandDelegate<T> method, CommandCanExecuteDelegate<T> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            if (_showWaitCursor)
            {
                MouseEx.SetCursor(Cursors.Wait, () =>
                {
                    Catch.ShowMessageBox(() =>
                    {
                        if (parameter is T variable)
                        {
                            _method.Invoke(variable);
                        }
                        else
                        {
                            _method.Invoke(default(T));
                        }
                    });
                });
            }
            else
            {
                Catch.ShowMessageBox(() =>
                {
                    if (parameter is T variable)
                    {
                        _method.Invoke(variable);
                    }
                    else
                    {
                        _method.Invoke(default(T));
                    }
                });
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            if (parameter is T variable)
            {
                return _canExecute(variable);
            }
            return _canExecute(default(T));
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }

    public class TargetCommand : ICommand
    {
        private readonly CommandCanExecuteDelegate _canExecute;
        private readonly CommandDelegate _method;
        private readonly bool _showWaitCursor = true;

        public TargetCommand(CommandDelegate method)
        {
            _method = method;
        }

        public TargetCommand(CommandDelegate method, bool showWaitCursor)
        {
            _method = method;
            _showWaitCursor = showWaitCursor;
        }

        public TargetCommand(CommandDelegate method, CommandCanExecuteDelegate canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public void Execute(object parameter)
        {
            //_method.Invoke();
            if (_showWaitCursor)
            {
                MouseEx.SetCursor(Cursors.Wait, () => { Catch.ShowMessageBox(() => _method.Invoke()); });
            }
            else
            {
                Catch.ShowMessageBox(() => _method.Invoke());
            }
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}