using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingsViewModel
    {
        private string _searchString = "";

        public List<Setting> Settings { get; } = new List<Setting>
        {
            new StringSetting(
                name: "Assembly name",
                initialValue: "ConsoleApp1",
                priority: 1,
                defaultValue: "ConsoleApp1",
                description: "Specifies the name of the generated assembly, both on the file system and in metadata."),
            new StringSetting(
                name: "Default namespace",
                initialValue: "ConsoleApp1",
                priority: 2,
                defaultValue: "ConsoleApp1",
                description: "Specifies the root namespace for the project, which controls code generation and analyzers."),
            new StringSetting(
                name: "Target frameworks",
                initialValue: ".NET Core 3.0",
                defaultValue: null,
                priority: 3,
                description: "Specifies the semicolon-delimited list of frameworks that this project will target. Often just a single value."),
            new EnumSetting(
                name: "Output type",
                initialValue: "Console Application",
                defaultValue: null,
                enumValues: new List<string> { "Console Application", "Windows Application", "Class Library" },
                priority: 4,
                description: "Specifies whether the output is executable, and whether is runs in a console or as a desktop application."),
            new BoolSetting(
                name: "Binding redirects",
                initialValue: true,
                defaultValue: true,
                description: "Whether to auto-generate binding redirects.",
                priority: 5),
            new MultiLineStringSetting(
                name: "Pre-build event",
                initialValue: "",
                defaultValue: "",
                priority: 6,
                description: "Commands to execute before a build occurs."),
            new MultiLineStringSetting(
                name: "Post-build event",
                initialValue: "",
                defaultValue: "",
                priority: 7,
                description: "Commands to execute after a build completes.")
        };

        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString == value)
                {
                    return;
                }

                _searchString = value;
                var view = CollectionViewSource.GetDefaultView(Settings);
                view.Filter = o => o is Setting setting && setting.MatchesSearchText(_searchString);
            }
        }

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