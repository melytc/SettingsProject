using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace SettingsProject
{
    internal static class SettingsLoader
    {
        static SettingsLoader()
        {
            var settingByIdentity = DefaultSettings.ToDictionary(setting => setting.Identity);

            foreach (var condition in DefaultConditions)
            {
                if (!settingByIdentity.TryGetValue(condition.Source, out Setting source))
                    throw new Exception("Unknown source: " + condition.Source);
                if (!settingByIdentity.TryGetValue(condition.Target, out Setting target))
                    throw new Exception("Unknown target: " + condition.Target);

                source.AddDependentTarget(target, condition.SourceValue);
            }
        }

        public static readonly IReadOnlyList<SettingCondition> DefaultConditions = new[]
        {
            // Multi-targeting
            new SettingCondition(
                source: new SettingIdentity("Application", "General", "Multi-targeting"),
                sourceValue: true,
                target: new SettingIdentity("Application", "General", "Target frameworks")),
            new SettingCondition(
                source: new SettingIdentity("Application", "General", "Multi-targeting"),
                sourceValue: false,
                target: new SettingIdentity("Application", "General", "Target framework")),
            // Resources
            new SettingCondition(
                source: new SettingIdentity("Application", "Resources", "Resources"),
                sourceValue: "Icon and manifest",
                target: new SettingIdentity("Application", "Resources", "Icon path")),
            new SettingCondition(
                source: new SettingIdentity("Application", "Resources", "Resources"),
                sourceValue: "Icon and manifest",
                target: new SettingIdentity("Application", "Resources", "Manifest path")),
            new SettingCondition(
                source: new SettingIdentity("Application", "Resources", "Resources"),
                sourceValue: "Resource file",
                target: new SettingIdentity("Application", "Resources", "Resource file path")),

            new SettingCondition(
                source: new SettingIdentity("Packaging", "General", "License specification"),
                sourceValue: "Expression",
                target: new SettingIdentity("Packaging", "General", "License expression")),
            new SettingCondition(
                source: new SettingIdentity("Packaging", "General", "License specification"),
                sourceValue: "Expression",
                target: new SettingIdentity("Packaging", "General", "Read about SPDX license expressions")),
            new SettingCondition(
                source: new SettingIdentity("Packaging", "General", "License specification"),
                sourceValue: "File",
                target: new SettingIdentity("Packaging", "General", "License file path")),

            new SettingCondition(
                source: new SettingIdentity("Debug", "General", "Launch type"),
                sourceValue: "Executable",
                target: new SettingIdentity("Debug", "General", "Executable path")),

            new SettingCondition(
                source: new SettingIdentity("Debug", "General", "Use remote machine"),
                sourceValue: true,
                target: new SettingIdentity("Debug", "General", "Remote machine host name")),
            new SettingCondition(
                source: new SettingIdentity("Debug", "General", "Use remote machine"),
                sourceValue: true,
                target: new SettingIdentity("Debug", "General", "Authentication mode")),

            new SettingCondition(
                source: new SettingIdentity("Signing", "General", "Signing"),
                sourceValue: true,
                target: new SettingIdentity("Signing", "General", "Key file path")),
            new SettingCondition(
                source: new SettingIdentity("Signing", "General", "Signing"),
                sourceValue: true,
                target: new SettingIdentity("Signing", "General", "Delay signing")),
        };

        public static readonly IReadOnlyList<Setting> DefaultSettings = new Setting[]
        {
            /////////////
            //////////// APPLICATION
            ///////////

            //////
            ///// GENERAL
            ////

            new StringSetting(
                name: "Assembly name",
                description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
                page: "Application",
                category: "General",
                priority: 10,
                new UnconfiguredStringSettingValue(initialValue: "ConsoleApp1", defaultValue: "ConsoleApp1")),
            new StringSetting(
                name: "Default namespace",
                description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                page: "Application",
                category: "General",
                priority: 200,
                new UnconfiguredStringSettingValue(initialValue: "ConsoleApp1", defaultValue: "ConsoleApp1")),
            new BoolSetting(
                name: "Multi-targeting",
                description: "Build this project for multiple target frameworks.",
                page: "Application",
                category: "General",
                priority: 300,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: null)),
            // TODO come up with a better editing experience, perhaps via a FlagsEnumSetting
            // TODO allow completion of values: new[] { ".net5", ".netcoreapp3.1", ".netcoreapp3.0", ".netcoreapp2.2", ".netcoreapp2.1", ".netcoreapp2.0", ".netcoreapp1.1", ".netcoreapp1.0" }
            new StringSetting(
                name: "Target frameworks",
                description:
                "Specifies the semicolon-delimited list of frameworks that this project will target.",
                page: "Application",
                category: "General",
                priority: 310,
                new UnconfiguredStringSettingValue(initialValue: "net5", defaultValue: null)),
            new EnumSetting(
                name: "Target framework",
                description: "Specifies the framework that this project will target.",
                page: "Application",
                category: "General",
                priority: 320,
                enumValues: new[]
                {
                    ".NET 5",
                    ".NET Core 3.1",
                    ".NET Core 3.0",
                    ".NET Core 2.2",
                    ".NET Core 2.1",
                    ".NET Core 2.0",
                    ".NET Core 1.1",
                    ".NET Core 1.0"
                },
                new UnconfiguredEnumSettingValue(initialValue: ".NET 5", defaultValue: null)),
            new LinkAction(
                name: "Install other frameworks",
                description: null,
                page: "Application",
                category: "General",
                priority: 400),
            new EnumSetting(
                name: "Output type",
                description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                page: "Application",
                category: "General",
                priority: 500,
                enumValues: new[]
                {
                    "Console Application",
                    "Windows Application",
                    "Class Library"
                },
                new UnconfiguredEnumSettingValue(initialValue: "Console Application", defaultValue: null)),
            new BoolSetting(
                name: "Binding redirects",
                description: "Whether to auto-generate binding redirects.",
                page: "Application",
                category: "General",
                priority: 600,
                new UnconfiguredBoolSettingValue(initialValue: true, defaultValue: true)),
            new EnumSetting(
                name: "Startup object",
                description: "Specifies the entry point for the executable.",
                page: "Application",
                category: "General",
                priority: 700,
                enumValues: new[] { "(Not set)" },
                new UnconfiguredEnumSettingValue(initialValue: "(Not set)", defaultValue: "(Not set)")),

            new EnumSetting(
                name: "Resources",
                description: "Specifies how application resources will be managed.",
                page: "Application",
                category: "Resources",
                priority: 800,
                enumValues: new[] { "Icon and manifest", "Resource file" },
                new UnconfiguredEnumSettingValue(initialValue: "Icon and manifest", defaultValue: "Icon and manifest")),
            // TODO make this IconBrowseSetting
            new StringSetting(
                name: "Icon path",
                description: "Path to the icon to embed into the output assembly.",
                page: "Application",
                category: "Resources",
                priority: 810,
                new UnconfiguredStringSettingValue(initialValue: "(Default Icon)", defaultValue: "(Default Icon)")),
            // TODO make this FileBrowseSetting
            // TODO this can appear disabled, find out why
            new EnumSetting(
                name: "Manifest path",
                description: "A manifest determines specific settings for an application. To embed a custom manifest, first add it to your project and then select it from the list.",
                page: "Application",
                category: "Resources",
                priority: 820,
                enumValues: new[] { "" },
                new UnconfiguredEnumSettingValue(initialValue: "", defaultValue: "")),
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "Resource file path",
                description: "Specifies a Win32 res file to compile into this project.",
                page: "Application",
                category: "Resources",
                priority: 830,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: "")),

            //////
            ///// ASSEMBLY INFORMATION
            ////

            // TODO this section is disabled for .NET Core projects -- if we have time, determine whether there's anything in here we couldn't add later

//            new StringSetting(
//                name: "Assembly name",
//                initialValue: "ConsoleApp1",
//                priority: 20,
//                defaultValue: "ConsoleApp1",
//                description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
//                page: "Application",
//                category: "Assembly Information"),

            // TODO consider other UI elements not listed here

            /////////////
            //////////// BUILD
            ///////////

            //////
            ///// GENERAL
            ////

            new StringSetting(
                name: "Conditional compilation symbols",
                description: "A semicolon-delimited list of symbols to define for the compilation.",
                page: "Build",
                category: "General",
                priority: 30,
                new UnconfiguredStringSettingValue(initialValue: "TRACE", defaultValue: null),
                supportsPerConfigurationValues: true),
            new BoolSetting(
                name: "Define DEBUG symbol",
                description: "Specifies whether to define the DEBUG compilation symbol.",
                page: "Build",
                category: "General",
                priority: 200,
                new ConfiguredBoolSettingValue("Debug | AnyCPU", initialValue: true, defaultValue: true),
                new ConfiguredBoolSettingValue("Release | AnyCPU", initialValue: false, defaultValue: false)),
            new BoolSetting(
                name: "Define TRACE symbol",
                description: "Specifies whether to define the TRACE compilation symbol.",
                page: "Build",
                category: "General",
                priority: 300,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false),
                supportsPerConfigurationValues: true),
            new EnumSetting(
                name: "Platform target",
                description: "The platform to target in this project configuration.",
                page: "Build",
                category: "General",
                priority: 400,
                enumValues: new[] { "Any CPU", "x86" },
                new UnconfiguredEnumSettingValue(initialValue: "Any CPU", defaultValue: "Any CPU")),
            new EnumSetting(
                name: "Nullable reference types",
                description: "Controls use of nullable annotations and warnings.",
                page: "Build",
                category: "General",
                priority: 500,
                enumValues: new[] { "Disable", "Enable", "Warnings", "Annotations" },
                new UnconfiguredEnumSettingValue(initialValue: "Enable", defaultValue: "Disable")),
            // TODO this is disabled in .NET Core -- why?
            new BoolSetting(
                name: "Prefer 32-bit",
                description: "Specifies whether to prefer 32-bit when available.",
                page: "Build",
                category: "General",
                priority: 600,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
            new BoolSetting(
                name: "Unsafe code",
                description: "Allow unsafe code in this project.",
                page: "Build",
                category: "General",
                priority: 700,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
            new BoolSetting(
                name: "Optimize code",
                description: "Produce optimized output. Optimized binaries may be harder to debug.",
                page: "Build",
                category: "General",
                priority: 800,
                new ConfiguredBoolSettingValue("Debug | AnyCPU", initialValue: false, defaultValue: false),
                new ConfiguredBoolSettingValue("Release | AnyCPU", initialValue: true, defaultValue: true)),

            //////
            ///// ERRORS AND WARNINGS
            ////
            
            new EnumSetting(
                name: "Warning level",
                description: "Sets the warning level, where higher levels produce more warnings.",
//              readMore: "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/warn-compiler-option",
                page: "Build",
                category: "Errors and warnings",
                priority: 40,
                enumValues: new[] { "0", "1", "2", "3", "4" },
                new UnconfiguredEnumSettingValue(initialValue: "4", defaultValue: "4")),
            new StringSetting(
                name: "Suppress specific warnings",
                description: "A semicolon-delimited list of warning codes to suppress.",
                page: "Build",
                category: "Errors and warnings",
                priority: 200,
                new UnconfiguredStringSettingValue(initialValue: "1701;1702", defaultValue: "1701;1702")),
            new EnumSetting(
                name: "Warnings as errors",
                description: "Controls which warnings are treated as errors.",
                page: "Build",
                category: "Errors and warnings",
                priority: 300,
                enumValues: new[] { "None", "All", "Specific warnings" },
                new UnconfiguredEnumSettingValue(initialValue: "Specific warnings", defaultValue: "Specific warnings")),
            new StringSetting(
                name: "Treat specific warnings as errors",
                description: "A semicolon-delimited list of warning codes to treat as errors.",
                page: "Build",
                category: "Errors and warnings",
                priority: 400,
                new UnconfiguredStringSettingValue(initialValue: "NU1605", defaultValue: "NU1605")),

            //////
            ///// OUTPUT
            ////
            
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "Output path",
                description: "Relative destination path for build output.",
                page: "Build",
                category: "Output",
                priority: 50,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: "")),
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "XML documentation path",
                description: "Relative path to the output XML documentation. Clear to disable generation.",
                page: "Build",
                category: "Output",
                priority: 200,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: "")),
            // TODO this is disabled in .NET Core -- why?
            new BoolSetting(
                name: "Register for COM interop",
                description: "Add metadata from the output assembly to the registry, allowing COM clients to create .NET classes.",
                page: "Build",
                category: "Output",
                priority: 300,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
            new EnumSetting(
                name: "Generate serialization assembly",
                description: null,
                page: "Build",
                category: "Output",
                priority: 400,
                enumValues: new[] { "Auto", "On", "Off" },
                new UnconfiguredEnumSettingValue(initialValue: "Auto", defaultValue: "Auto")),

            //////
            ///// ADVANCED
            ////
            
            new LinkAction(
                name: "Language version",
                description: "Why can't I select the C# language version?",
                page: "Build",
                category: "Advanced",
                priority: 60),

            new EnumSetting(
                name: "Internal compiler error reporting",
                description: null,
                page: "Build",
                category: "Advanced",
                priority: 200,
                enumValues: new[] { "None", "Prompt", "Send", "Queue" },
                new UnconfiguredEnumSettingValue(initialValue: "Prompt", defaultValue: "Prompt")),
            new BoolSetting(
                name: "Overflow checking",
                description: "Enable arithmetic overflow checking at runtime.",
                page: "Build",
                category: "Advanced",
                priority: 300,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false),
                supportsPerConfigurationValues: true),
            new EnumSetting(
                name: "Debugging information",
                description: null,
                page: "Build",
                category: "Advanced",
                priority: 400,
                enumValues: new[] { "None", "Full", "Pdb-only", "Portable", "Embedded" },
                new UnconfiguredEnumSettingValue(initialValue: "Portable", defaultValue: "Portable"),
                supportsPerConfigurationValues: true),
            new EnumSetting(
                name: "File alignment",
                description: null,
                page: "Build",
                category: "Advanced",
                priority: 500,
                enumValues: new[] { "512", "1024", "2048", "4096", "8192" },
                new UnconfiguredEnumSettingValue(initialValue: "512", defaultValue: "512")),
            new StringSetting(
                name: "Library base address",
                description: null,
                page: "Build",
                category: "Advanced",
                priority: 600,
                new UnconfiguredStringSettingValue(initialValue: "0x11000000", defaultValue: "0x11000000")),

            /////////////
            //////////// BUILD EVENTS
            ///////////

            //////
            ///// GENERAL
            ////

            // TODO both these build events can be edited in a pop-out editor with macro support
            new MultiLineStringSetting(
                name: "Pre-build event",
                description: "Commands to execute before a build occurs.",
                page: "Build Events",
                category: "General",
                priority: 70,
                new UnconfiguredMultilineStringSettingValue(initialValue: "", defaultValue: "")),
            new MultiLineStringSetting(
                name: "Post-build event",
                description: "Commands to execute after a build completes.",
                page: "Build Events",
                category: "General",
                priority: 200,
                new UnconfiguredMultilineStringSettingValue(initialValue: "", defaultValue: "")),
            new EnumSetting(
                name: "Run the post-build event",
                description: "Controls when any post-build event is executed.",
                page: "Build Events",
                category: "General",
                priority: 300,
                enumValues: new[]
                {
                    "Always",
                    "On successful build",
                    "When the build updates the project output"
                },
                new UnconfiguredEnumSettingValue(
                    initialValue: "On successful build",
                    defaultValue: "On successful build")),

            /////////////
            //////////// PACKAGING
            ///////////

            //////
            ///// GENERAL
            ////

            new BoolSetting(
                name: "Generate NuGet package on build",
                description: "Specifies whether a NuGet package should be produced in the output directory when the project is build.",
                page: "Packaging",
                category: "General",
                priority: 80,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false),
                supportsPerConfigurationValues: true),
            new BoolSetting(
                name: "Require license acceptance",
                description: "Controls whether consumers of the generated package are presented with a license acceptance prompt when adding a reference to this package.",
                page: "Packaging",
                category: "General",
                priority: 200,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
            new StringSetting(
                name: "Package ID",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 300,
                new UnconfiguredStringSettingValue(initialValue: "ConsoleApp1", defaultValue: "ConsoleApp1")),
            // TODO VersionSetting (note -- has different validation rules to assembly/file versions)
            new StringSetting(
                name: "Package version",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 400,
                new UnconfiguredStringSettingValue(initialValue: "1.0.0", defaultValue: "1.0.0")),
            new StringSetting(
                name: "Authors",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 500,
                new UnconfiguredStringSettingValue(initialValue: "ConsoleApp1", defaultValue: "ConsoleApp1")),
            new StringSetting(
                name: "Company",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 600,
                new UnconfiguredStringSettingValue(initialValue: "ConsoleApp1", defaultValue: "ConsoleApp1")),
            new StringSetting(
                name: "Product",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 700,
                new UnconfiguredStringSettingValue(initialValue: "ConsoleApp1", defaultValue: "ConsoleApp1")),
            new MultiLineStringSetting(
                name: "Description",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 800,
                new UnconfiguredMultilineStringSettingValue(initialValue: "", defaultValue: "")),
            new StringSetting(
                name: "Copyright",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 900,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new EnumSetting(
                name: "License specification",
                description: "Controls how the package's license is specified.",
                page: "Packaging",
                category: "General",
                priority: 905,
                enumValues: new[] { "None", "Expression", "File" },
                new UnconfiguredEnumSettingValue(initialValue: "None", defaultValue: "None")),
            // TODO provide some examples for auto-complete: Apache-2.0;MIT;...
            new StringSetting(
                name: "License expression",
                description: "The SPDX expression that specifies the package's license.",
                page: "Packaging",
                category: "General",
                priority: 910,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new LinkAction(
                // https://spdx.org/licenses/
                name: "Read about SPDX license expressions",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 920),
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "License file path",
                description: "The path to the license file to include in the package. May be relative to the project directory.",
                page: "Packaging",
                category: "General",
                priority: 930,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            // TODO make this IconBrowseSetting
            new StringSetting(
                name: "Package icon file",
                description: "Path to the icon to include in and use for the package.",
                page: "Packaging",
                category: "General",
                priority: 1100,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new StringSetting(
                name: "Repository URL",
                description: null, // TODO describe what this URL means
                page: "Packaging",
                category: "General",
                priority: 1200,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            // TODO provide feedback about valid URLs here
            new StringSetting(
                name: "Repository type",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 1300,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new StringSetting(
                name: "Tags",
                description: null, // TODO describe how this is delimited
                page: "Packaging",
                category: "General",
                priority: 1400,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new MultiLineStringSetting(
                name: "Release notes",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 1500,
                new UnconfiguredMultilineStringSettingValue(initialValue: "", defaultValue: "")),
            // TODO this is a combo box with many languages listed
            new StringSetting(
                name: "Assembly neutral language",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 1600,
                new UnconfiguredStringSettingValue(initialValue: "(None)", defaultValue: "(None)")),
            // TODO VersionSetting
            new StringSetting(
                name: "Assembly version",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 1700,
                new UnconfiguredStringSettingValue(initialValue: "1.0.0.0", defaultValue: "1.0.0.0")),
            // TODO VersionSetting
            new StringSetting(
                name: "Assembly file version",
                description: null,
                page: "Packaging",
                category: "General",
                priority: 1800,
                new UnconfiguredStringSettingValue(initialValue: "1.0.0.0", defaultValue: "1.0.0.0")),

            /////////////
            //////////// PACKAGING
            ///////////

            //////
            ///// GENERAL
            ////
            
            new EnumSetting(
                name: "Launch type",
                description: null,
                page: "Debug",
                category: "General",
                priority: 90,
                enumValues: new[] { "Project", "Executable" },
                new UnconfiguredEnumSettingValue(initialValue: "Project", defaultValue: "Project")),
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "Executable path",
                description: "Path to the executable to debug.",
                page: "Debug",
                category: "General",
                priority: 100,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new StringSetting(
                name: "Application arguments",
                description: "Arguments to be passed to the launched application.",
                page: "Debug",
                category: "General",
                priority: 200,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "Working directory",
                description: "Absolute path to the working directory.",
                page: "Debug",
                category: "General",
                priority: 300,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new BoolSetting(
                name: "Use remote machine",
                description: "The debug target is on a remote machine.",
                page: "Debug",
                category: "General",
                priority: 400,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
            // TODO make this RemoteMachineSetting, with support for the 'Find' button
            new StringSetting(
                name: "Remote machine host name",
                description: null,
                page: "Debug",
                category: "General",
                priority: 410,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null)),
            new EnumSetting(
                name: "Authentication mode",
                description: null,
                page: "Debug",
                category: "General",
                priority: 420,
                enumValues: new[] { "None", "Windows" },
                new UnconfiguredEnumSettingValue(initialValue: "None", defaultValue: "None")),
            // TODO NameValueListSetting
            new StringSetting(
                name: "Environment variables",
                description: "Specifies environment variables to be set for the launched application.",
                page: "Debug",
                category: "General",
                priority: 500,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: "")),
            new BoolSetting(
                name: "Native code debugging",
                description: "Enable native code debugging.",
                page: "Debug",
                category: "General",
                priority: 600,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
            new BoolSetting(
                name: "SQL Server debugging",
                description: "Enable SQL Server debugging.",
                page: "Debug",
                category: "General",
                priority: 700,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),

            /////////////
            //////////// SIGNING
            ///////////

            //////
            ///// GENERAL
            ////
            
            new BoolSetting(
                name: "Signing",
                description: "Sign the project's output assembly.",
                page: "Signing",
                category: "General",
                priority: 92,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false),
                supportsPerConfigurationValues: true),
            // TODO StrongNameKeySetting -- with new/add and change password actions
            new StringSetting(
                name: "Key file path",
                description: "Choose a string name key file",
                page: "Signing",
                category: "General",
                priority: 110,
                new UnconfiguredStringSettingValue(initialValue: "", defaultValue: null),
                supportsPerConfigurationValues: true),
            new BoolSetting(
                name: "Delay signing",
                description: "Delay sign the assembly. When enabled the project will not run or be debuggable.",
                page: "Signing",
                category: "General",
                priority: 120,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false),
                supportsPerConfigurationValues: true),

            /////////////
            //////////// CODE ANALYSIS
            ///////////

            //////
            ///// ANALYZERS
            ////
            
            new LinkAction(
                name: "What are the benefits of source code analyzers?",
                description: null,
                page: "Code Analysis",
                category: "Analyzers",
                priority: 94),
            new BoolSetting(
                name: "Run on build",
                description: "Run analyzers during build.",
                page: "Code Analysis",
                category: "Analyzers",
                priority: 200,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false),
                supportsPerConfigurationValues: true),
            new BoolSetting(
                name: "Run live analysis",
                description: "Run analyzers live in the IDE.",
                page: "Code Analysis",
                category: "Analyzers",
                priority: 300,
                new UnconfiguredBoolSettingValue(initialValue: false, defaultValue: false)),
       };
    }
}