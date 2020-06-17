using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    abstract class Setting
    {
        public string Name { get; }

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        protected Setting(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
    }

    abstract class Setting<T> : Setting, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        private readonly T _defaultValue;
        private T _value;

        //public string? Description { get; }

        /// <summary>
        /// An optional comparer to use when evaluating value change events.
        /// </summary>
        /// <remarks>
        /// If an override does not exist, or returns <see langword="null"/>,
        /// the <see cref="EqualityComparer{T}.Default"/> is used.
        /// </remarks>
        public virtual IEqualityComparer<T>? Comparer => null;

        /// <summary>
        /// Gets and sets the current value of the property.
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                var comparer = Comparer ?? EqualityComparer<T>.Default;

                if (!comparer.Equals(value, Value))
                {
                    // Only raise event when Value actually changes
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }

            }
        }

#pragma warning disable CS8618 // _value is not initialized.
        protected Setting(string name, T initialValue, T defaultValue, int priority)
#pragma warning restore CS8618 // _value is not initialized.
            : base(name, priority)
        {
            _defaultValue = defaultValue;
            Value = initialValue;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class StringSetting : Setting<string>
    {
        public override IEqualityComparer<string>? Comparer { get; }

        public StringSetting(string name, string initialValue, string defaultValue, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue, priority)
        {
            Comparer = comparer;
        }
    }

    class MultiLineStringSetting : Setting<string>
    {
        public override IEqualityComparer<string>? Comparer { get; }

        public MultiLineStringSetting(string name, string initialValue, string defaultValue, int priority, IEqualityComparer<string>? comparer = null)
            : base(name, initialValue, defaultValue, priority)
        {
            Comparer = comparer;
        }
    }

    class SettingsViewModel
    {
        public List<Setting> Settings { get; } = new List<Setting>
        {
            new StringSetting(
                name: "Assembly name",
                initialValue: "ConsoleApp1",
                priority: 1,
                defaultValue: "ConsoleApp1"),
            new StringSetting(
                name: "Default namespace",
                initialValue: "ConsoleApp1",
                priority: 2,
                defaultValue: "ConsoleApp1"),
            new StringSetting(
                name: "Target framework",
                initialValue: ".NET Code 3.0",
                defaultValue: "",
                priority: 3),
            new EnumSetting(
                name: "Output type",
                initialValue: "Console Application",
                defaultValue: "Console Application",
                enumValues: new List<string> { "Console Application", "Windows Application", "Class Library" },
                priority: 4),
            new BoolSetting(
                name: "Binding redirects",
                initialValue: true,
                defaultValue: true,
                description: "Auto-generate binding redirects",
                priority: 5),
            new MultiLineStringSetting(
                name: "Pre-build event",
                initialValue: "",
                defaultValue: "",
                priority: 6)
        };

        public SettingsViewModel()
        {
            // Construct the default view for our settings collection and customise it.
            // When the view binds the collection, it will use this pre-constructed view.
            // We will be able to use this view for filtering too (search, advanced mode, etc).
            var view = CollectionViewSource.GetDefaultView(Settings);

            // Specify the property to sort on, and direction to sort.
            view.SortDescriptions.Add(new SortDescription(nameof(Setting.Priority), ListSortDirection.Ascending));
        }
    }

    class BoolSetting : Setting<bool>
    {
        public BoolSetting(string name, bool initialValue, bool defaultValue, string description, int priority)
            : base(name, initialValue, defaultValue, priority)
        {
            Description = description;
        }

        public string Description { get; }
    }

    class EnumSetting : Setting<string>
    {
        public List<string> EnumValues { get; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, string initialValue, string defaultValue, List<string> enumValues, int priority)
            : base(name, initialValue, defaultValue, priority)
        {
            EnumValues = enumValues;
        }
    }
}
