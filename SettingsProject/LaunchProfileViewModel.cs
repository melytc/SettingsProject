using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable

namespace SettingsProject
{
    internal sealed class LaunchProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isRenaming;
        private string _name;

        public LaunchProfileKind Kind { get; }

        public SettingsListViewModel SettingsListViewModel { get; }

        public LaunchProfileViewModel(string name, ImmutableArray<Setting> settings, LaunchProfileKind kind)
        {
            SettingsListViewModel = new SettingsListViewModel(settings, useGrouping: false);

            _name = name;
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
            var context = new SettingContext();
            var settings = SettingsListViewModel.Settings.Select(setting => setting.Clone(context)).ToImmutableArray();

            foreach (var setting in settings)
            {
                setting.UpdateDependentVisibilities();
            }

            return new LaunchProfileViewModel($"{Name} (2)", settings, Kind);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
