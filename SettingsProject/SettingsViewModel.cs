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
                description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
                page: "Application",
                category: "General"),
            new StringSetting(
                name: "Default namespace",
                initialValue: "ConsoleApp1",
                priority: 2,
                defaultValue: "ConsoleApp1",
                description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                page: "Application",
                category: "General"),
            new EnumSetting(
                name: "Target framework",
                initialValue: ".NET 5",
                defaultValue: null,
                priority: 3,
                description: "Specifies the semicolon-delimited list of frameworks that this project will target. Often just a single value.",
                page: "Application",
                category: "General",
                enumValues: new List<string> { ".NET 5", ".NET Core 3.1", ".NET Core 3.0", ".NET Core 2.2", ".NET Core 2.1", ".NET Core 2.0", ".NET Core 1.1", ".NET Core 1.0" }),
            new LinkAction(
                text: "Install other frameworks",
                priority: 3,
                page: "Application",
                category: "General"),
            new EnumSetting(
                name: "Output type",
                initialValue: "Console Application",
                defaultValue: null,
                enumValues: new List<string> { "Console Application", "Windows Application", "Class Library" },
                priority: 4,
                description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Binding redirects",
                initialValue: true,
                defaultValue: true,
                description: "Whether to auto-generate binding redirects.",
                priority: 5,
                page: "Application",
                category: "General"),
            new EnumSetting(
                name: "Startup object",
                initialValue: "(Not set)",
                defaultValue: "(Not set)",
                enumValues: new List<string> { "(Not set)" },
                priority:64,
                description: "Specifies the entry point for the executable.",
                page: "Application",
                category: "General"),
            // TODO both these build events can be edited in a pop-out editor with macro support
            new MultiLineStringSetting(
                name: "Pre-build event",
                initialValue: "",
                defaultValue: "",
                priority: 6,
                description: "Commands to execute before a build occurs.",
                page: "Build Events",
                category: "General"),
            new MultiLineStringSetting(
                name: "Post-build event",
                initialValue: "",
                defaultValue: "",
                priority: 7,
                description: "Commands to execute after a build completes.",
                page: "Build Events",
                category: "General"),
            new EnumSetting(
                name: "Run the post-build event",
                initialValue: "On successful build",
                defaultValue: "On successful build",
                enumValues: new List<string> { "Always", "On successful build", "When the build updates the project output" },
                priority: 4,
                description: "Controls when any post-build event is executed.",
                page: "Build Events",
                category: "General"),
            new StringSetting(
                name: "Conditional compilation symbols",
                initialValue: "TRACE",
                defaultValue: null,
                priority: 8,
                description: "A semicolon-delimited list of symbols to define for the compilation.",
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Define DEBUG symbol",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to define the DEBUG compilation symbol.",
                priority: 9,
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Define TRACE symbol",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to define the TRACE compilation symbol.",
                priority: 10,
                page: "Application",
                category: "General"),
            new EnumSetting(
                name: "Platform target",
                initialValue: "Any CPU",
                defaultValue: "Any CPU",
                enumValues: new List<string> { "Any CPU", "x86" },
                priority: 11,
                description: "The platform to target in this project configuration.",
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Prefer 32-bit",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to prefer 32-bit when available.",
                priority: 12,
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Unsafe code",
                initialValue: false,
                defaultValue: false,
                description: "Allow unsafe code in this project.",
                priority: 13,
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Optimize code",
                initialValue: false,
                defaultValue: false,
                description: "Produce optimized output. Optimized binaries may be harder to debug.",
                priority: 14,
                page: "Application",
                category: "General"),
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

                var trimmed = value.Trim();
                var view = CollectionViewSource.GetDefaultView(Settings);

                if (string.IsNullOrEmpty(trimmed))
                {
                    view.Filter = null;
                }
                else
                {
                    view.Filter = o => o is Setting setting && setting.MatchesSearchText(trimmed);
                }
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

            if (view.CanGroup)
            {
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Category)));
            }
        }
    }
}