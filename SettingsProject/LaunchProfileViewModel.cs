using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class LaunchProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isRenaming;
        private string _name;
        private readonly PropertyContext _context;

        public LaunchProfileKind Kind { get; }

        public IReadOnlyList<Property> Properties => _context.Properties;

        public LaunchProfileViewModel(string name, LaunchProfileKind kind, PropertyContext context)
        {
            _name = name;
            _context = context;
            Kind = kind;
         
            PropertyListViewSource.Initialize(Properties, useGrouping: false);
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
            // TODO improve this strategy, as the suffix may not be unique, or the name may already have a numerical suffix
            return new LaunchProfileViewModel($"{Name} (2)", Kind, _context.Clone());
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
