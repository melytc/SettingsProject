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
                description: "Commands to execute after a build completes."),
            new StringSetting(
                name: "Conditional compilation symbols",
                initialValue: "TRACE",
                defaultValue: null,
                priority: 8,
                description: "A semicolon-delimited list of symbols to define for the compilation."),
            new BoolSetting(
                name: "Define DEBUG symbol",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to define the DEBUG compilation symbol.",
                priority: 9),
            new BoolSetting(
                name: "Define TRACE symbol",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to define the TRACE compilation symbol.",
                priority: 10),
            new EnumSetting(
                name: "Platform target",
                initialValue: "Any CPU",
                defaultValue: "Any CPU",
                enumValues: new List<string> { "Any CPU", "x86" },
                priority: 11,
                description: "The platform to target in this project configuration."),
            new BoolSetting(
                name: "Prefer 32-bit",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to prefer 32-bit when available.",
                priority: 12),
            new BoolSetting(
                name: "Unsafe code",
                initialValue: false,
                defaultValue: false,
                description: "Allow unsafe code in this project.",
                priority: 13),
            new BoolSetting(
                name: "Optimize code",
                initialValue: false,
                defaultValue: false,
                description: "Produce optimized output. Optimized binaries may be harder to debug.",
                priority: 14),
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