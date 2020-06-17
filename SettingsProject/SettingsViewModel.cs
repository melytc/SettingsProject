using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingsViewModel
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
}