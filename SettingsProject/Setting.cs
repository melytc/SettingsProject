using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SettingsProject
{
#nullable enable
    abstract class Setting
    {
        public string Name { get; }

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        //public string? Description { get; }

        protected Setting(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
    }

    class StringSetting : Setting
    {
        public StringSetting(string name, string value, int priority) : base(name, priority)
        {
            Value = value;
        }

        public string Value { get; set; }
    }

    class MultiLineStringSetting : Setting
    {
        public MultiLineStringSetting(string name, string value, int priority) : base(name, priority)
        {
            Value = value;
        }

        public string Value { get; set; }
    }

    class SettingsViewModel
    {
        public List<Setting> Settings { get; } = new List<Setting>
        {
            new StringSetting("Assembly name", "ConsoleApp1", priority: 1),
            new StringSetting("Default namespace", "ConsoleApp1", priority: 2),
            new StringSetting("Target framework", ".NET Code 3.0", priority: 3),
            new EnumSetting("Output type", new List<string>{ "Console Application", "Windows Application", "Class Library" }, priority: 4),
            new BoolSetting("Binding redirects", true, "Auto-generate binding redirects", priority: 5),
            new MultiLineStringSetting("Pre-build event", "", priority: 6)
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

    class BoolSetting : Setting
    {
        public BoolSetting(string name, bool value, string description, int priority) : base(name, priority)
        {
            Value = value;
            Description = description;
        }

        public bool Value { get; set; }
        public string Description { get; }
    }

    class EnumSetting : Setting
    {
        public List<string> EnumValues { get; }
        public string SelectedValue { get; set; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, List<string> enumValues, int priority) : base(name, priority)
        {
            EnumValues = enumValues;
            SelectedValue = enumValues[0];
        }
    }
}
