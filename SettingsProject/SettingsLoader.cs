using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

                source.AddDependentTarget(target.Identity, condition.SourceValue);
            }
        }

        // TODO control 'Prefer 32-bit' visibility based on target framework(s)

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
                source: new SettingIdentity("Packaging", "License", "License specification"),
                sourceValue: "Expression",
                target: new SettingIdentity("Packaging", "License", "License expression")),
            new SettingCondition(
                source: new SettingIdentity("Packaging", "License", "License specification"),
                sourceValue: "Expression",
                target: new SettingIdentity("Packaging", "License", "Read about SPDX license expressions")),
            new SettingCondition(
                source: new SettingIdentity("Packaging", "License", "License specification"),
                sourceValue: "File",
                target: new SettingIdentity("Packaging", "License", "License file path")),

            new SettingCondition(
                source: new SettingIdentity("Signing", "General", "Signing"),
                sourceValue: true,
                target: new SettingIdentity("Signing", "General", "Key file path")),
            new SettingCondition(
                source: new SettingIdentity("Signing", "General", "Signing"),
                sourceValue: true,
                target: new SettingIdentity("Signing", "General", "Delay signing")),
        };

        public static readonly SettingContext DefaultContext = new SettingContext();

        public static readonly IReadOnlyList<Setting> DefaultSettings = new Setting[]
        {
            /////////////
            //////////// APPLICATION
            ///////////

            //////
            ///// GENERAL
            ////

            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Assembly name",
                    description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
                    page: "Application",
                    category: "General",
                    priority: 10,
                    editorType: "String"),
                new UnconfiguredSettingValue("ConsoleApp1")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Default namespace",
                    description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                    page: "Application",
                    category: "General",
                    priority: 200,
                    editorType: "String"),
                new UnconfiguredSettingValue("ConsoleApp1")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Multi-targeting",
                    description: "Build this project for multiple target frameworks.",
                    page: "Application",
                    category: "General",
                    priority: 300,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(false)),
            // TODO come up with a better editing experience, perhaps via a FlagsSetting
            // TODO allow completion of values: new[] { ".net5", ".netcoreapp3.1", ".netcoreapp3.0", ".netcoreapp2.2", ".netcoreapp2.1", ".netcoreapp2.0", ".netcoreapp1.1", ".netcoreapp1.0" }
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Target frameworks",
                    description: "Specifies the semicolon-delimited list of frameworks that this project will target.",
                    page: "Application",
                    category: "General",
                    priority: 310,
                    editorType: "String"),
                new UnconfiguredSettingValue("net5")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Target framework",
                    description: "Specifies the framework that this project will target.",
                    page: "Application",
                    category: "General",
                    priority: 320,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create(
                        ".NET 5",
                        ".NET Core 3.1",
                        ".NET Core 3.0",
                        ".NET Core 2.2",
                        ".NET Core 2.1",
                        ".NET Core 2.0",
                        ".NET Core 1.1",
                        ".NET Core 1.0")
                },
                new UnconfiguredSettingValue(".NET 5")),
            new LinkAction(
                context: DefaultContext,
                name: "Install other frameworks",
                description: null,
                page: "Application",
                category: "General",
                priority: 400),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Output type",
                    description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                    page: "Application",
                    category: "General",
                    priority: 500,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create(
                        "Console Application",
                        "Windows Application",
                        "Class Library")
                },
                new UnconfiguredSettingValue("Console Application")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Binding redirects",
                    description: "Whether to auto-generate binding redirects.",
                    page: "Application",
                    category: "General",
                    priority: 600,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(true)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Startup object",
                    description: "Specifies the entry point for the executable.",
                    page: "Application",
                    category: "General",
                    priority: 700,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("(Not set)"),
                },
                value: new UnconfiguredSettingValue("(Not set)")),

            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Resources",
                    description: "Specifies how application resources will be managed.",
                    page: "Application",
                    category: "Resources",
                    priority: 800,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("Icon and manifest", "Resource file"),
                },
                value: new UnconfiguredSettingValue("Icon and manifest")),
            // TODO make this IconBrowseSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Icon path",
                    description: "Path to the icon to embed into the output assembly.",
                    page: "Application",
                    category: "Resources",
                    priority: 810,
                    editorType: "String"),
                new UnconfiguredSettingValue("(Default Icon)")),
            // TODO make this FileBrowseSetting
            // TODO this can appear disabled, find out why
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Manifest path",
                    description: "A manifest determines specific settings for an application. To embed a custom manifest, first add it to your project and then select it from the list.",
                    page: "Application",
                    category: "Resources",
                    priority: 820,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create(""),
                },
                value: new UnconfiguredSettingValue("")),
            // TODO make this FileBrowseSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Resource file path",
                    description: "Specifies a Win32 res file to compile into this project.",
                    page: "Application",
                    category: "Resources",
                    priority: 830,
                    editorType: "String"),
                value: new UnconfiguredSettingValue("")),

            //////
            ///// ASSEMBLY INFORMATION
            ////

            // TODO this section is disabled for .NET Core projects -- if we have time, determine whether there's anything in here we couldn't add later

//            new Setting(
//                context: DefaultContext,
//                name: "Assembly name",
//                initialValue: "ConsoleApp1",
//                priority: 20,
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

            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Conditional compilation symbols",
                    description: "A semicolon-delimited list of symbols to define for the compilation.",
                    page: "Build",
                    category: "General",
                    priority: 30,
                    editorType: "String")
                {
                    SupportsPerConfigurationValues = true
                },
                value: new UnconfiguredSettingValue("TRACE")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Define DEBUG symbol",
                    description: "Specifies whether to define the DEBUG compilation symbol.",
                    page: "Build",
                    category: "General",
                    priority: 200,
                    editorType: "Bool"),
                values: ImmutableArray.Create<ISettingValue>(
                    new ConfiguredSettingValue("Debug | AnyCPU", value: true),
                    new ConfiguredSettingValue("Release | AnyCPU", value: false))),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Define TRACE symbol",
                    description: "Specifies whether to define the TRACE compilation symbol.",
                    page: "Build",
                    category: "General",
                    priority: 300,
                    editorType: "Bool")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Platform target",
                    description: "The platform to target in this project configuration.",
                    page: "Build",
                    category: "General",
                    priority: 400,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("Any CPU", "x86")
                },
                value: new UnconfiguredSettingValue("Any CPU")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Nullable reference types",
                    description: "Controls use of nullable annotations and warnings.",
                    page: "Build",
                    category: "General",
                    priority: 500,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("Disable", "Enable", "Warnings", "Annotations")
                },
                value: new UnconfiguredSettingValue("Enable")),
            // TODO this is disabled in .NET Core -- why?
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Prefer 32-bit",
                    description: "Specifies whether to prefer 32-bit when available.",
                    page: "Build",
                    category: "General",
                    priority: 600,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Unsafe code",
                    description: "Allow unsafe code in this project.",
                    page: "Build",
                    category: "General",
                    priority: 700,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Optimize code",
                    description: "Produce optimized output. Optimized binaries may be harder to debug.",
                    page: "Build",
                    category: "General",
                    priority: 800,
                    editorType: "Bool"),
                values: ImmutableArray.Create<ISettingValue>(
                    new ConfiguredSettingValue("Debug | AnyCPU", value: false),
                    new ConfiguredSettingValue("Release | AnyCPU", value: true))),

            //////
            ///// ERRORS AND WARNINGS
            ////
            
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Warning level",
                    description: "Sets the warning level, where higher levels produce more warnings.",
    //              readMore: "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/warn-compiler-option",
                    page: "Build",
                    category: "Errors and warnings",
                    priority: 40,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("0", "1", "2", "3", "4")
                },
                value: new UnconfiguredSettingValue("4")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Suppress specific warnings",
                    description: "A semicolon-delimited list of warning codes to suppress.",
                    page: "Build",
                    category: "Errors and warnings",
                    priority: 200,
                    editorType: "String"),
                new UnconfiguredSettingValue("1701;1702")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Warnings as errors",
                    description: "Controls which warnings are treated as errors.",
                    page: "Build",
                    category: "Errors and warnings",
                    priority: 300,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("None", "All", "Specific warnings"),
                },
                value: new UnconfiguredSettingValue("Specific warnings")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Treat specific warnings as errors",
                    description: "A semicolon-delimited list of warning codes to treat as errors.",
                    page: "Build",
                    category: "Errors and warnings",
                    priority: 400,
                    editorType: "String"),
                new UnconfiguredSettingValue("NU1605")),

            //////
            ///// OUTPUT
            ////
            
            // TODO make this FileBrowseSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Output path",
                    description: "Relative destination path for build output.",
                    page: "Build",
                    category: "Output",
                    priority: 50,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            // TODO make this FileBrowseSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "XML documentation path",
                    description: "Relative path to the output XML documentation. Clear to disable generation.",
                    page: "Build",
                    category: "Output",
                    priority: 200,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            // TODO this is disabled in .NET Core -- why?
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Register for COM interop",
                    description: "Add metadata from the output assembly to the registry, allowing COM clients to create .NET classes.",
                    page: "Build",
                    category: "Output",
                    priority: 300,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Generate serialization assembly",
                    description: null,
                    page: "Build",
                    category: "Output",
                    priority: 400,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("Auto", "On", "Off")
                },
                value: new UnconfiguredSettingValue("Auto")),

            //////
            ///// ADVANCED
            ////
            
            new LinkAction(
                context: DefaultContext,
                name: "Language version",
                description: "Why can't I select the C# language version?",
                page: "Build",
                category: "Advanced",
                priority: 60),

            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Internal compiler error reporting",
                    description: null,
                    page: "Build",
                    category: "Advanced",
                    priority: 200,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("None", "Prompt", "Send", "Queue"),
                },
                value: new UnconfiguredSettingValue("Prompt")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Overflow checking",
                    description: "Enable arithmetic overflow checking at runtime.",
                    page: "Build",
                    category: "Advanced",
                    priority: 300,
                    editorType: "Bool")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Debugging information",
                    description: null,
                    page: "Build",
                    category: "Advanced",
                    priority: 400,
                    editorType: "Enum")
                {
                    SupportsPerConfigurationValues = true,
                    EnumValues = ImmutableArray.Create("None", "Full", "Pdb-only", "Portable", "Embedded")
                },
                value: new UnconfiguredSettingValue("Portable")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "File alignment",
                    description: null,
                    page: "Build",
                    category: "Advanced",
                    priority: 500,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("512", "1024", "2048", "4096", "8192")
                },
                value: new UnconfiguredSettingValue("512")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Library base address",
                    description: null,
                    page: "Build",
                    category: "Advanced",
                    priority: 600,
                    editorType: "String"),
                new UnconfiguredSettingValue("0x11000000")),

            /////////////
            //////////// BUILD EVENTS
            ///////////

            //////
            ///// GENERAL
            ////

            // TODO both these build events can be edited in a pop-out editor with macro support
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Pre-build event",
                    description: "Commands to execute before a build occurs.",
                    page: "Build Events",
                    category: "General",
                    priority: 70,
                    editorType: "MultiLineString"),
                value: new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Post-build event",
                    description: "Commands to execute after a build completes.",
                    page: "Build Events",
                    category: "General",
                    priority: 200,
                    editorType: "MultiLineString"),
                value: new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Run the post-build event",
                    description: "Controls when any post-build event is executed.",
                    page: "Build Events",
                    category: "General",
                    priority: 300,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create(
                        "Always",
                        "On successful build",
                        "When the build updates the project output")
                },
                value: new UnconfiguredSettingValue("On successful build")),

            /////////////
            //////////// PACKAGING
            ///////////

            //////
            ///// GENERAL
            ////

            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Generate NuGet package on build",
                    description: "Specifies whether a NuGet package should be produced in the output directory when the project is build.",
                    page: "Packaging",
                    category: "General",
                    priority: 80,
                    editorType: "Bool")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Package ID",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 300,
                    editorType: "String"),
                new UnconfiguredSettingValue("ConsoleApp1")),
            // TODO VersionSetting (note -- has different validation rules to assembly/file versions)
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Package version",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 400,
                    editorType: "String"),
                new UnconfiguredSettingValue("1.0.0")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Authors",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 500,
                    editorType: "String"),
                new UnconfiguredSettingValue("ConsoleApp1")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Company",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 600,
                    editorType: "String"),
                new UnconfiguredSettingValue("ConsoleApp1")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Product",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 700,
                    editorType: "String"),
                new UnconfiguredSettingValue("ConsoleApp1")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Description",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 800,
                    editorType: "MultiLineString"),
                new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Copyright",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 900,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            // TODO make this IconBrowseSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Package icon file",
                    description: "Path to the icon to include in and use for the package.",
                    page: "Packaging",
                    category: "General",
                    priority: 1100,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Repository URL",
                    description: null, // TODO describe what this URL means
                    page: "Packaging",
                    category: "General",
                    priority: 1200,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            // TODO provide feedback about valid URLs here
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Repository type",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1300,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Tags",
                    description: null, // TODO describe how this is delimited
                    page: "Packaging",
                    category: "General",
                    priority: 1400,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Release notes",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1500,
                    editorType: "MultiLineString"),
                new UnconfiguredSettingValue("")),
            // TODO this is a combo box with many languages listed
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Assembly neutral language",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1600,
                    editorType: "String"),
                new UnconfiguredSettingValue("(None)")),
            // TODO VersionSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Assembly version",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1700,
                    editorType: "String"),
                new UnconfiguredSettingValue("1.0.0.0")),
            // TODO VersionSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Assembly file version",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1800,
                    editorType: "String"),
                new UnconfiguredSettingValue("1.0.0.0")),

            //////
            ///// LICENSE
            ////
            
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Require license acceptance",
                    description: "Controls whether consumers of the generated package are presented with a license acceptance prompt when adding a reference to this package.",
                    page: "Packaging",
                    category: "License",
                    priority: 85,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "License specification",
                    description: "Controls how the package's license is specified.",
                    page: "Packaging",
                    category: "License",
                    priority: 200,
                    editorType: "Enum")
                {
                    EnumValues = ImmutableArray.Create("None", "Expression", "File")
                },
                value: new UnconfiguredSettingValue("None")),
            // TODO provide some examples for auto-complete: Apache-2.0;MIT;...
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "License expression",
                    description: "The SPDX expression that specifies the package's license.",
                    page: "Packaging",
                    category: "License",
                    priority: 300,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),
            new LinkAction(
                context: DefaultContext,
                // https://spdx.org/licenses/
                name: "Read about SPDX license expressions",
                description: null,
                page: "Packaging",
                category: "License",
                priority: 400),
            // TODO make this FileBrowseSetting
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "License file path",
                    description: "The path to the license file to include in the package. May be relative to the project directory.",
                    page: "Packaging",
                    category: "License",
                    priority: 500,
                    editorType: "String"),
                new UnconfiguredSettingValue("")),

            /////////////
            //////////// DEBUG
            ///////////

            //////
            ///// GENERAL
            ////
            
            // TODO make this link action show the launch profiles UI
            new LinkAction(
                context: DefaultContext,
                name: "Manage launch profiles",
                description: null,
                page: "Debug",
                category: "General",
                priority: 90),

            /////////////
            //////////// SIGNING
            ///////////

            //////
            ///// GENERAL
            ////
            
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Signing",
                    description: "Sign the project's output assembly.",
                    page: "Signing",
                    category: "General",
                    priority: 92,
                    editorType: "Bool")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue(false)),
            // TODO StrongNameKeySetting -- with new/add and change password actions
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Key file path",
                    description: "Choose a string name key file",
                    page: "Signing",
                    category: "General",
                    priority: 110,
                    editorType: "String")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue("")),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Delay signing",
                    description: "Delay sign the assembly. When enabled the project will not run or be debuggable.",
                    page: "Signing",
                    category: "General",
                    priority: 120,
                    editorType: "Bool")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue(false)),

            /////////////
            //////////// CODE ANALYSIS
            ///////////

            //////
            ///// ANALYZERS
            ////
            
            new LinkAction(
                context: DefaultContext,
                name: "What are the benefits of source code analyzers?",
                description: null,
                page: "Code Analysis",
                category: "Analyzers",
                priority: 94),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Run on build",
                    description: "Run analyzers during build.",
                    page: "Code Analysis",
                    category: "Analyzers",
                    priority: 200,
                    editorType: "Bool")
                {
                    SupportsPerConfigurationValues = true
                },
                new UnconfiguredSettingValue(false)),
            new Setting(
                context: DefaultContext,
                new SettingMetadata(
                    name: "Run live analysis",
                    description: "Run analyzers live in the IDE.",
                    page: "Code Analysis",
                    category: "Analyzers",
                    priority: 300,
                    editorType: "Bool"),
                new UnconfiguredSettingValue(false)),
       };
    }
}