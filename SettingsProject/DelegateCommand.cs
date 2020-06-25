using System;
using System.Windows.Input;

#nullable enable

namespace SettingsProject
{
    internal sealed class DelegateCommand : ICommand
    {
        private readonly Action _action;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();

        event EventHandler? ICommand.CanExecuteChanged { add { } remove { } }
    }
}