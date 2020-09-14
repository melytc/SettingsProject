using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace SettingsProject
{
    internal sealed class LaunchProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isRenaming;
        private string _name;
        private readonly SettingContext _context;

        public LaunchProfileKind Kind { get; }

        public SettingsListViewModel SettingsListViewModel { get; }

        public LaunchProfileViewModel(string name, LaunchProfileKind kind, SettingContext context)
        {
            SettingsListViewModel = new SettingsListViewModel(context.Settings, useGrouping: false);

            _name = name;
            _context = context;
            Kind = kind;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (!string.Equals(_name, value, StringComparison.Ordinal))
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRenaming
        {
            get => _isRenaming;
            set
            {
                if (_isRenaming != value)
                {
                    _isRenaming = value;
                    OnPropertyChanged();
                }
            }
        }

        public LaunchProfileViewModel Clone()
        {
            return new LaunchProfileViewModel($"{Name} (2)", Kind, _context.Clone());
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
