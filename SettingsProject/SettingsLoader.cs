using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft;

#nullable enable

namespace SettingsProject
{
    internal static class SettingsLoader
    {
        private static readonly ImmutableDictionary<string, string> DebugConfiguration = new Dictionary<string, string> {{"Configuration", "Debug"}}.ToImmutableDictionary(StringComparers.ConfigurationDimensionNames);
        private static readonly ImmutableDictionary<string, string> ReleaseConfiguration = new Dictionary<string, string> {{"Configuration", "Release"}}.ToImmutableDictionary(StringComparers.ConfigurationDimensionNames);

        // TODO control 'Prefer 32-bit' visibility based on target framework(s)

        private static readonly ImmutableArray<SettingCondition> DefaultConditions = ImmutableArray.Create(
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
                target: new SettingIdentity("Signing", "General", "Delay signing"))
        );

        private static readonly IImmutableDictionary<string, ImmutableArray<string>> DefaultConfigurationDictionary = new Dictionary<string, ImmutableArray<string>>
        {
            { "Configuration", ImmutableArray.Create("Debug", "Release") },
            { "Platform", ImmutableArray.Create("x86", "AnyCPU") },
            { "Target Framework", ImmutableArray.Create("net5.0", "net472") },
        }.ToImmutableDictionary(StringComparers.ConfigurationDimensionNames);

        public static SettingContext CreateDefaultContext() => new SettingContext(
            DefaultConfigurationDictionary,
            DefaultConditions,
            ImmutableArray.Create(

                /////////////
                //////////// APPLICATION
                ///////////

                //////
                ///// GENERAL
                ////

                new Setting(
                    new SettingMetadata(
                        name: "Assembly name",
                        description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
                        page: "Application",
                        category: "General",
                        priority: 10,
                        editorType: "String"),
                    new SettingValue("$(ProjectName)", "ConsoleApp1")),
                new Setting(
                    new SettingMetadata(
                        name: "Default namespace",
                        description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                        page: "Application",
                        category: "General",
                        priority: 200,
                        editorType: "String"),
                    new SettingValue("$(DefaultNamespace)", "ConsoleApp1")),
                new Setting(
                    new SettingMetadata(
                        name: "Multi-targeting",
                        description: "Build this project for multiple target frameworks.",
                        page: "Application",
                        category: "General",
                        priority: 300,
                        editorType: "Bool"),
                    new SettingValue("false", false)),
                // TODO come up with a better editing experience, perhaps via a FlagsSetting
                // TODO allow completion of values: new[] { ".net5", ".netcoreapp3.1", ".netcoreapp3.0", ".netcoreapp2.2", ".netcoreapp2.1", ".netcoreapp2.0", ".netcoreapp1.1", ".netcoreapp1.0" }
                new Setting(
                    new SettingMetadata(
                        name: "Target frameworks",
                        description: "Specifies the semicolon-delimited list of frameworks that this project will target.",
                        page: "Application",
                        category: "General",
                        priority: 310,
                        editorType: "String"),
                    new SettingValue("net5", "net5")),
                new Setting(
                    new SettingMetadata(
                        name: "Target framework",
                        description: "Specifies the framework that this project will target.",
                        page: "Application",
                        category: "General",
                        priority: 320,
                        editorType: "Enum"),
                    new SettingValue("net5.0", "net5.0")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue(".NET 5", "net5.0"),
                            new SupportedValue(".NET Core 3.1", "netcoreapp3.1"),
                            new SupportedValue(".NET Core 3.0", "netcoreapp3.0"),
                            new SupportedValue(".NET Core 2.2", "netcoreapp2.2"),
                            new SupportedValue(".NET Core 2.1", "netcoreapp2.1"),
                            new SupportedValue(".NET Core 2.0", "netcoreapp2.0"),
                            new SupportedValue(".NET Core 1.1", "netcoreapp1.1"),
                            new SupportedValue(".NET Core 1.0", "netcoreapp1.0"))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Install other frameworks",
                        description: null,
                        page: "Application",
                        category: "General",
                        priority: 400,
                        editors: ImmutableArray.Create(
                            new EditorSpecification(
                                "LinkAction",
                                new Dictionary<string, string>
                                {
                                    {"Action", "URL"},
                                    {"URL", "http://go.microsoft.com/fwlink/?LinkID=287120"}
                                }))),
                    values: ImmutableArray<SettingValue>.Empty),
                new Setting(
                    new SettingMetadata(
                        name: "Output type",
                        description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                        page: "Application",
                        category: "General",
                        priority: 500,
                        editorType: "Enum"),
                    new SettingValue("Exe", "Exe")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Console Application", "Exe"),
                            new SupportedValue("Windows Application", "WinExe"),
                            new SupportedValue("Class Library", ""))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Binding redirects",
                        description: "Whether to auto-generate binding redirects.",
                        page: "Application",
                        category: "General",
                        priority: 600,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new SettingValue("true", true)),
                new Setting(
                    new SettingMetadata(
                        name: "Startup object",
                        description: "Specifies the entry point for the executable.",
                        page: "Application",
                        category: "General",
                        priority: 700,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("(Not set)", "(Not set)")
                    {
                        SupportedValues = ImmutableArray.Create(new SupportedValue("(Not set)")),
                    }),

                new Setting(
                    new SettingMetadata(
                        name: "Resources",
                        description: "Specifies how application resources will be managed.",
                        page: "Application",
                        category: "Resources",
                        priority: 800,
                        editorType: "Enum"),
                    value: new SettingValue("Icon and manifest", "Icon and manifest")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Icon and manifest"),
                            new SupportedValue("Resource file")),
                    }),
                // TODO make this IconBrowseSetting
                new Setting(
                    new SettingMetadata(
                        name: "Icon path",
                        description: "Path to the icon to embed into the output assembly.",
                        page: "Application",
                        category: "Resources",
                        priority: 810,
                        editorType: "String"),
                    new SettingValue("(Default Icon)", "(Default Icon)")),
                // TODO make this FileBrowseSetting
                // TODO this can appear disabled, find out why
                new Setting(
                    new SettingMetadata(
                        name: "Manifest path",
                        description: "A manifest determines specific settings for an application. To embed a custom manifest, first add it to your project and then select it from the list.",
                        page: "Application",
                        category: "Resources",
                        priority: 820,
                        editorType: "Enum"),
                    value: new SettingValue("", "")
                    {
                        SupportedValues = ImmutableArray.Create(new SupportedValue(""))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Resource file path",
                        description: "Specifies a Win32 res file to compile into this project.",
                        page: "Application",
                        category: "Resources",
                        priority: 830,
                        editorType: "FileBrowse"),
                    value: new SettingValue("", "")),

                //////
                ///// ASSEMBLY INFORMATION
                ////

                // TODO this section is disabled for .NET Core projects -- if we have time, determine whether there's anything in here we couldn't add later

//                new Setting(
//                    context: DefaultContext,
//                    name: "Assembly name",
//                    initialValue: "ConsoleApp1",
//                    priority: 20,
//                    description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
//                    page: "Application",
//                    category: "Assembly Information"),

                // TODO consider other UI elements not listed here

                /////////////
                //////////// BUILD
                ///////////

                //////
                ///// GENERAL
                ////

                new Setting(
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
                    value: new SettingValue("TRACE", "TRACE")),
                new Setting(
                    new SettingMetadata(
                        name: "Define DEBUG symbol",
                        description: "Specifies whether to define the DEBUG compilation symbol.",
                        page: "Build",
                        category: "General",
                        priority: 200,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    values: ImmutableArray.Create(
                        new SettingValue("true", evaluatedValue: true, configurationDimensions: DebugConfiguration),
                        new SettingValue("false", evaluatedValue: false, configurationDimensions: ReleaseConfiguration))),
                new Setting(
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
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Platform target",
                        description: "The platform to target in this project configuration.",
                        page: "Build",
                        category: "General",
                        priority: 400,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("Any CPU", "Any CPU")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Any CPU"),
                            new SupportedValue("x86"))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Nullable reference types",
                        description: "Controls use of nullable annotations and warnings.",
                        page: "Build",
                        category: "General",
                        priority: 500,
                        editorType: "Enum"),
                    value: new SettingValue("enable", "Enable")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Disable"),
                            new SupportedValue("Enable"),
                            new SupportedValue("Warnings"),
                            new SupportedValue("Annotations"))
                    }),
                // TODO this is disabled in .NET Core -- why?
                new Setting(
                    new SettingMetadata(
                        name: "Prefer 32-bit",
                        description: "Specifies whether to prefer 32-bit when available.",
                        page: "Build",
                        category: "General",
                        priority: 600,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Unsafe code",
                        description: "Allow unsafe code in this project.",
                        page: "Build",
                        category: "General",
                        priority: 700,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Optimize code",
                        description: "Produce optimized output. Optimized binaries may be harder to debug.",
                        page: "Build",
                        category: "General",
                        priority: 800,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    values: ImmutableArray.Create(
                        new SettingValue("false", evaluatedValue: false, configurationDimensions: DebugConfiguration),
                        new SettingValue("true", evaluatedValue: true, configurationDimensions: ReleaseConfiguration))),

                //////
                ///// ERRORS AND WARNINGS
                ////

                new Setting(
                    new SettingMetadata(
                        name: "Warning level",
                        description: "Sets the warning level, where higher levels produce more warnings.",
                        //              readMore: "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/warn-compiler-option",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 40,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("4", "4")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("0"),
                            new SupportedValue("1"),
                            new SupportedValue("2"),
                            new SupportedValue("3"),
                            new SupportedValue("4"))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Suppress specific warnings",
                        description: "A semicolon-delimited list of warning codes to suppress.",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 200,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new SettingValue("1701;1702", "1701;1702")),
                new Setting(
                    new SettingMetadata(
                        name: "Warnings as errors",
                        description: "Controls which warnings are treated as errors.",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 300,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("Specific warnings", "Specific warnings")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("All"),
                            new SupportedValue("Specific warnings"))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Treat specific warnings as errors",
                        description: "A semicolon-delimited list of warning codes to treat as errors.",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 400,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new SettingValue("NU1605", "NU1605")),

                //////
                ///// OUTPUT
                ////

                new Setting(
                    new SettingMetadata(
                        name: "Output path",
                        description: "Relative destination path for build output.",
                        page: "Build",
                        category: "Output",
                        priority: 50,
                        editorType: "FileBrowse")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    ImmutableArray.Create(
                        new SettingValue("bin\\$(Configuration)", "bin\\Debug", DebugConfiguration),
                        new SettingValue("bin\\$(Configuration)", "bin\\Release", ReleaseConfiguration))),
                new Setting(
                    new SettingMetadata(
                        name: "XML documentation path",
                        description: "Relative path to the output XML documentation. Clear to disable generation.",
                        page: "Build",
                        category: "Output",
                        priority: 200,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    ImmutableArray.Create(
                        new SettingValue("$(OutputPath)\\$(ProjectName).xml", "bin\\Debug\\ConsoleApp1.xml", DebugConfiguration),
                        new SettingValue("$(OutputPath)\\$(ProjectName).xml", "bin\\Release\\ConsoleApp1.xml", ReleaseConfiguration))),
                // TODO this is disabled in .NET Core -- why?
                new Setting(
                    new SettingMetadata(
                        name: "Register for COM interop",
                        description: "Add metadata from the output assembly to the registry, allowing COM clients to create .NET classes.",
                        page: "Build",
                        category: "Output",
                        priority: 300,
                        editorType: "Bool"),
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Generate serialization assembly",
                        description: null,
                        page: "Build",
                        category: "Output",
                        priority: 400,
                        editorType: "Enum"),
                    value: new SettingValue("Auto", "Auto")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Auto"),
                            new SupportedValue("On"),
                            new SupportedValue("Off"))
                    }),

                //////
                ///// ADVANCED
                ////

                new Setting(
                    new SettingMetadata(
                        name: "Language version",
                        description: "Why can't I select the C# language version?",
                        page: "Build",
                        category: "Advanced",
                        priority: 60,
                        editors: ImmutableArray.Create(
                            new EditorSpecification(
                                "LinkAction",
                                new Dictionary<string, string>
                                {
                                    {"Action", "URL"},
                                    {"URL", "https://aka.ms/csharp-versions"}
                                }))),
                    values: ImmutableArray<SettingValue>.Empty),

                new Setting(
                    new SettingMetadata(
                        name: "Internal compiler error reporting",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 200,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("Prompt", "Prompt")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("Prompt"),
                            new SupportedValue("Send"),
                            new SupportedValue("Queue"))
                    }),
                new Setting(
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
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Debugging information",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 400,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("Portable", "Portable")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("Full"),
                            new SupportedValue("Pdb-only"),
                            new SupportedValue("Portable"),
                            new SupportedValue("Embedded"))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "File alignment",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 500,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("512", "512")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("512"),
                            new SupportedValue("1024"),
                            new SupportedValue("2048"),
                            new SupportedValue("4096"),
                            new SupportedValue("8192"))
                    }),
                new Setting(
                    new SettingMetadata(
                        name: "Library base address",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 600,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new SettingValue("0x11000000", "0x11000000")),

                /////////////
                //////////// BUILD EVENTS
                ///////////

                //////
                ///// GENERAL
                ////

                // TODO both these build events can be edited in a pop-out editor with macro support
                new Setting(
                    new SettingMetadata(
                        name: "Pre-build event",
                        description: "Commands to execute before a build occurs.",
                        page: "Build Events",
                        category: "General",
                        priority: 70,
                        editorType: "MultiLineString"),
                    value: new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Post-build event",
                        description: "Commands to execute after a build completes.",
                        page: "Build Events",
                        category: "General",
                        priority: 200,
                        editorType: "MultiLineString"),
                    value: new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Run the post-build event",
                        description: "Controls when any post-build event is executed.",
                        page: "Build Events",
                        category: "General",
                        priority: 300,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new SettingValue("On successful build", "On successful build")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Always"),
                            new SupportedValue("On successful build"),
                            new SupportedValue("When the build updates the project output"))
                    }),

                /////////////
                //////////// PACKAGING
                ///////////

                //////
                ///// GENERAL
                ////

                new Setting(
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
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Package ID",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 300,
                        editorType: "String"),
                    new SettingValue("$(ProjectName)", "ConsoleApp1")),
                // TODO VersionSetting (note -- has different validation rules to assembly/file versions)
                new Setting(
                    new SettingMetadata(
                        name: "Package version",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 400,
                        editorType: "String"),
                    new SettingValue("$(Version)", "1.0.0")),
                new Setting(
                    new SettingMetadata(
                        name: "Authors",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 500,
                        editorType: "String"),
                    new SettingValue("$(ProjectName)", "ConsoleApp1")),
                new Setting(
                    new SettingMetadata(
                        name: "Company",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 600,
                        editorType: "String"),
                    new SettingValue("$(ProjectName)", "ConsoleApp1")),
                new Setting(
                    new SettingMetadata(
                        name: "Product",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 700,
                        editorType: "String"),
                    new SettingValue("$(ProjectName)", "ConsoleApp1")),
                new Setting(
                    new SettingMetadata(
                        name: "Description",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 800,
                        editorType: "MultiLineString"),
                    new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Copyright",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 900,
                        editorType: "String"),
                    new SettingValue("", "")),
                // TODO make this IconBrowseSetting
                new Setting(
                    new SettingMetadata(
                        name: "Package icon file",
                        description: "Path to the icon to include in and use for the package.",
                        page: "Packaging",
                        category: "General",
                        priority: 1100,
                        editorType: "String"),
                    new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Repository URL",
                        description: null, // TODO describe what this URL means
                        page: "Packaging",
                        category: "General",
                        priority: 1200,
                        editorType: "String"),
                    new SettingValue("", "")),
                // TODO provide feedback about valid URLs here
                new Setting(
                    new SettingMetadata(
                        name: "Repository type",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1300,
                        editorType: "String"),
                    new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Tags",
                        description: null, // TODO describe how this is delimited
                        page: "Packaging",
                        category: "General",
                        priority: 1400,
                        editorType: "String"),
                    new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Release notes",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1500,
                        editorType: "MultiLineString"),
                    new SettingValue("", "")),
                // TODO this is a combo box with many languages listed
                new Setting(
                    new SettingMetadata(
                        name: "Assembly neutral language",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1600,
                        editorType: "String"),
                    new SettingValue("(None)", "(None)")),
                // TODO VersionSetting
                new Setting(
                    new SettingMetadata(
                        name: "Assembly version",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1700,
                        editorType: "String"),
                    new SettingValue("1.0.0.0", "1.0.0.0")),
                // TODO VersionSetting
                new Setting(
                    new SettingMetadata(
                        name: "Assembly file version",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1800,
                        editorType: "String"),
                    new SettingValue("1.0.0.0", "1.0.0.0")),

                //////
                ///// LICENSE
                ////

                new Setting(
                    new SettingMetadata(
                        name: "Require license acceptance",
                        description: "Controls whether consumers of the generated package are presented with a license acceptance prompt when adding a reference to this package.",
                        page: "Packaging",
                        category: "License",
                        priority: 85,
                        editorType: "Bool"),
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "License specification",
                        description: "Controls how the package's license is specified.",
                        page: "Packaging",
                        category: "License",
                        priority: 200,
                        editorType: "Enum"),
                    value: new SettingValue("None", "None")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("Expression"),
                            new SupportedValue("File"))
                    }),
                // TODO provide some examples for auto-complete: Apache-2.0;MIT;...
                new Setting(
                    new SettingMetadata(
                        name: "License expression",
                        description: "The SPDX expression that specifies the package's license.",
                        page: "Packaging",
                        category: "License",
                        priority: 300,
                        editorType: "String"),
                    new SettingValue("", "")),
                new Setting(
                    new SettingMetadata(
                        name: "Read about SPDX license expressions",
                        description: null,
                        page: "Packaging",
                        category: "License",
                        priority: 400,
                        editors: ImmutableArray.Create(
                            new EditorSpecification(
                                "LinkAction",
                                new Dictionary<string, string>
                                {
                                    {"Action", "URL"},
                                    {"URL", "https://spdx.org/licenses/"}
                                }))),
                    values: ImmutableArray<SettingValue>.Empty),
                new Setting(
                    new SettingMetadata(
                        name: "License file path",
                        description: "The path to the license file to include in the package. May be relative to the project directory.",
                        page: "Packaging",
                        category: "License",
                        priority: 500,
                        editorType: "FileBrowse"),
                    new SettingValue("", "")),

                /////////////
                //////////// DEBUG
                ///////////

                //////
                ///// GENERAL
                ////

                // TODO make this link action show the launch profiles UI
                new Setting(
                    new SettingMetadata(
                        name: "Manage launch profiles",
                        description: null,
                        page: "Debug",
                        category: "General",
                        priority: 90,
                        editors: ImmutableArray.Create(
                            new EditorSpecification(
                                "LinkAction",
                                new Dictionary<string, string>
                                {
                                    {"Action", "Command"},
                                    {"Command", "ManageLaunchProfiles"}
                                }))),
                    values: ImmutableArray<SettingValue>.Empty),

                /////////////
                //////////// SIGNING
                ///////////

                //////
                ///// GENERAL
                ////

                new Setting(
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
                    new SettingValue("false", false)),
                // TODO StrongNameKeySetting -- with new/add and change password actions
                new Setting(
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
                    new SettingValue("", "")),
                new Setting(
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
                    new SettingValue("false", false)),

                /////////////
                //////////// CODE ANALYSIS
                ///////////

                //////
                ///// ANALYZERS
                ////

                new Setting(
                    new SettingMetadata(
                        name: "What are the benefits of source code analyzers?",
                        description: null,
                        page: "Code Analysis",
                        category: "Analyzers",
                        priority: 94,
                        editors: ImmutableArray.Create(
                            new EditorSpecification(
                                "LinkAction",
                                new Dictionary<string, string>
                                {
                                    {"Action", "URL"},
                                    {"URL", "https://docs.microsoft.com/visualstudio/code-quality/roslyn-analyzers-overview"}
                                }))),
                    values: ImmutableArray<SettingValue>.Empty),
                new Setting(
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
                    new SettingValue("false", false)),
                new Setting(
                    new SettingMetadata(
                        name: "Run live analysis",
                        description: "Run analyzers live in the IDE.",
                        page: "Code Analysis",
                        category: "Analyzers",
                        priority: 300,
                        editorType: "Bool"),
                    new SettingValue("false", false))
            ));

        #region SettingMetadata

        private static readonly SettingMetadata ExecutablePath = new SettingMetadata(
            name: "Executable path",
            description: "Path to the executable to debug.",
            page: "Debug",
            category: "General",
            priority: 100,
            editorType: "FileBrowse");

        private static readonly SettingMetadata ApplicationArguments = new SettingMetadata(
            name: "Application arguments",
            description: "Arguments to be passed to the launched application.",
            page: "Debug",
            category: "General",
            priority: 200,
            editorType: "String");

        private static readonly SettingMetadata WorkingDirectory = new SettingMetadata(
            name: "Working directory",
            description: "Absolute path to the working directory.",
            page: "Debug",
            category: "General",
            priority: 300,
            editorType: "FileBrowse");

        private static readonly SettingMetadata UseRemoteMachine = new SettingMetadata(
            name: "Use remote machine",
            description: "The debug target is on a remote machine.",
            page: "Debug",
            category: "General",
            priority: 400,
            editorType: "Bool");

        private static readonly SettingMetadata RemoteMachineHostName = new SettingMetadata(
            name: "Remote machine host name",
            description: null,
            page: "Debug",
            category: "General",
            priority: 410,
            editorType: "String"); // TODO RemoteMachineSetting, with support for the 'Find' button

        private static readonly SettingMetadata AuthenticationMode = new SettingMetadata(
            name: "Authentication mode",
            description: null,
            page: "Debug",
            category: "General",
            priority: 420,
            editorType: "Enum");

        private static readonly SettingMetadata LaunchBrowser = new SettingMetadata(
            name: "Launch Browser",
            description: "Whether a browser should be launched when this profile is invoked.",
            page: "Debug",
            category: "General",
            priority: 550,
            editorType: "Bool");

        private static readonly SettingMetadata LaunchBrowserUrl = new SettingMetadata(
            name: "Launch Browser URL",
            description: "Absolute or relative URL to direct the browser to when launched.",
            page: "Debug",
            category: "General",
            priority: 560,
            editorType: "String");

        private static readonly SettingMetadata EnvironmentVariables = new SettingMetadata(
            name: "Environment variables",
            description: "Specifies environment variables to be set for the launched application.",
            page: "Debug",
            category: "General",
            priority: 500,
            editorType: "String"); // TODO NameValueList

        private static readonly SettingMetadata NativeCodeDebugging = new SettingMetadata(
            name: "Native code debugging",
            description: "Enable native code debugging.",
            page: "Debug",
            category: "General",
            priority: 600,
            editorType: "Bool");

        private static readonly SettingMetadata SqlServerDebugging = new SettingMetadata(
            name: "SQL Server debugging",
            description: "Enable SQL Server debugging.",
            page: "Debug",
            category: "General",
            priority: 700,
            editorType: "Bool");

        private static readonly SettingMetadata AzureResource = new SettingMetadata(
            name: "Azure resource",
            description: "The Azure resource to use in your snapshot debugging session.",
            page: "Debug",
            category: "General",
            priority: 100,
            editorType: "Enum"); // TODO AzureResource

        private static readonly SettingMetadata AzureStorageAccount = new SettingMetadata(
            name: "Azure Storage account",
            description: "The Azure resource to use in your snapshot debugging session.",
            page: "Debug",
            category: "General",
            priority: 200,
            editorType: "Enum"); // TODO editorType AzureStorage

        private static readonly SettingMetadata AppUrl = new SettingMetadata(
            name: "App URL",
            description: "The URL at which the application will be hosted when running.",
            page: "Debug",
            category: "Web Server Settings",
            priority: 1100,
            editorType: "String");

        private static readonly SettingMetadata IisExpressBitness = new SettingMetadata(
            name: "IIS Express Bitness",
            description: "Bitness of the IIS Express process to launch (x86, x64).",
            page: "Debug",
            category: "Web Server Settings",
            priority: 1200,
            editorType: "Enum");

        private static readonly SettingMetadata HostingModel = new SettingMetadata(
            name: "Hosting Model",
            description: "The URL at which the application will be hosted when running.",
            page: "Debug",
            category: "Web Server Settings",
            priority: 1300,
            editorType: "Enum");

        private static readonly SettingMetadata EnableSSL = new SettingMetadata(
            name: "Enable SSL",
            description: null,
            page: "Debug",
            category: "Web Server Settings",
            priority: 1400,
            editorType: "Bool");

        private static readonly SettingMetadata EnableAnonymousAuthentication = new SettingMetadata(
            name: "Enable Anonymous Authentication",
            description: null,
            page: "Debug",
            category: "Web Server Settings",
            priority: 1500,
            editorType: "Bool");

        private static readonly SettingMetadata EnableWindowsAuthentication = new SettingMetadata(
            name: "Enable Windows Authentication",
            description: null,
            page: "Debug",
            category: "Web Server Settings",
            priority: 1600,
            editorType: "Bool");

        #endregion

        public static LaunchProfilesWindowViewModel CreateLaunchProfiles()
        {

            var remoteMachineConditions = ImmutableArray.Create(
                new SettingCondition(
                    source: UseRemoteMachine.Identity,
                    sourceValue: true,
                    target: RemoteMachineHostName.Identity),
                new SettingCondition(
                    source: UseRemoteMachine.Identity,
                    sourceValue: true,
                    target: AuthenticationMode.Identity));

            var launchBrowserConditions = ImmutableArray.Create(
                new SettingCondition(
                    source: LaunchBrowser.Identity,
                    sourceValue: true,
                    target: LaunchBrowserUrl.Identity));

            var executableKindSettingMetadata = ImmutableArray.Create(
                ExecutablePath,
                ApplicationArguments,
                WorkingDirectory,
                UseRemoteMachine,
                RemoteMachineHostName,
                AuthenticationMode,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging);

            var projectKindSettingMetadata = ImmutableArray.Create(
                ApplicationArguments,
                WorkingDirectory,
                UseRemoteMachine,
                RemoteMachineHostName,
                AuthenticationMode,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging);

            var snapshotDebuggerKindSettingMetadata = ImmutableArray.Create(
                AzureResource,
                AzureStorageAccount);

            var iisExpressKindSettingMetadata = ImmutableArray.Create(
                ApplicationArguments,
                WorkingDirectory,
                LaunchBrowser,
                LaunchBrowserUrl,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging,
                // Web server settings
                AppUrl,
                IisExpressBitness,
                HostingModel,
                EnableSSL,
                EnableAnonymousAuthentication,
                EnableWindowsAuthentication);

            var iisKindSettingMetadata = ImmutableArray.Create(
                ApplicationArguments,
                WorkingDirectory,
                LaunchBrowser,
                LaunchBrowserUrl,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging,
                // Web server settings
                AppUrl,
                IisExpressBitness,
                HostingModel,
                EnableSSL,
                EnableAnonymousAuthentication,
                EnableWindowsAuthentication);

            var projectKind = new LaunchProfileKind("Project", projectKindSettingMetadata, remoteMachineConditions, FindDrawing("IconApplicationDrawing"));
            var executableKind = new LaunchProfileKind("Executable", executableKindSettingMetadata, remoteMachineConditions, FindDrawing("IconExecuteDrawing"));
            var snapshotDebuggerKind = new LaunchProfileKind("Snapshot Debugger", snapshotDebuggerKindSettingMetadata, ImmutableArray<SettingCondition>.Empty, FindDrawing("SnapshotDebuggerDrawing"));
            var iisKind = new LaunchProfileKind("IIS", iisKindSettingMetadata, launchBrowserConditions, FindDrawing("IISDrawing"));
            var iisExpressKind = new LaunchProfileKind("IIS Express", iisExpressKindSettingMetadata, launchBrowserConditions, FindDrawing("IISExpressDrawing"));

            var supportedValuesBySetting = new Dictionary<SettingIdentity, ImmutableArray<SupportedValue>>
            {
                { AuthenticationMode.Identity, ImmutableArray.Create(new SupportedValue("None"), new SupportedValue("Windows")) },
                { IisExpressBitness.Identity, ImmutableArray.Create(new SupportedValue("Default"), new SupportedValue("x64"), new SupportedValue("x86")) },
                { HostingModel.Identity, ImmutableArray.Create(new SupportedValue("Default (In Process)"), new SupportedValue("In Process"), new SupportedValue("Out of Process")) }
            };

            var defaultValueByEditorType = new Dictionary<string, (string Unevaluated, object Evaluated)>
            {
                {"String", ("", "")},
                {"MultiLineString", ("", "")},
                {"Bool", ("false", false)},
                {"Enum", ("", "")},
                {"FileBrowse", ("", "")},
                {"LinkAction", ("", "")}
            };

            var profileKinds = ImmutableArray.Create(projectKind, executableKind, snapshotDebuggerKind, iisKind, iisExpressKind);

            var profiles = new ObservableCollection<LaunchProfileViewModel>
            {
                CreateLaunchProfileViewModel("My project", projectKind, new Dictionary<SettingIdentity, (string Unevaluated, object Evaluated)>
                {
                    { ApplicationArguments.Identity, ("/foo /bar", "/foo /bar") }
                }),
                CreateLaunchProfileViewModel("devenv.exe", executableKind, new Dictionary<SettingIdentity, (string Unevaluated, object Evaluated)>
                {
                    { ExecutablePath.Identity, ("devenv.exe", "devenv.exe") },
                    { ApplicationArguments.Identity, ("/rootSuffix Exp", "/rootSuffix Exp") }
                }),
                CreateLaunchProfileViewModel("My Snapshot", snapshotDebuggerKind, new Dictionary<SettingIdentity, (string Unevaluated, object Evaluated)>
                {
                    // TODO
                }),
                CreateLaunchProfileViewModel("My IIS", iisKind, new Dictionary<SettingIdentity, (string Unevaluated, object Evaluated)>
                {
                    { AppUrl.Identity, ("http://localhost:52531", "http://localhost:52531") },
                    { LaunchBrowser.Identity, ("true", true) }
                }),
                CreateLaunchProfileViewModel("My IIS Express", iisExpressKind, new Dictionary<SettingIdentity, (string Unevaluated, object Evaluated)>
                {
                    { AppUrl.Identity, ("http://localhost:52531", "http://localhost:52531") },
                    { LaunchBrowser.Identity, ("true", true) }
                })
            };

            return new LaunchProfilesWindowViewModel(profiles, profileKinds);

            LaunchProfileViewModel CreateLaunchProfileViewModel(string name, LaunchProfileKind kind, Dictionary<SettingIdentity, (string Unevaluated, object Evaluated)> initialValues)
            {
                var context = new SettingContext(
                    ImmutableDictionary<string, ImmutableArray<string>>.Empty,
                    kind.Conditions,
                    kind.Metadata.Select(CreateSetting).ToImmutableArray());

                return new LaunchProfileViewModel(name, kind, context);

                Setting CreateSetting(SettingMetadata metadata)
                {
                    // Debug launch profile values are unconfigured
                    var settingValue = new SettingValue(unevaluatedValue: "", evaluatedValue: "");

                    if (supportedValuesBySetting.TryGetValue(metadata.Identity, out ImmutableArray<SupportedValue> supportedValues))
                    {
                        settingValue.SupportedValues = supportedValues;
                        settingValue.EvaluatedValue = supportedValues.First();
                    }

                    if (initialValues.TryGetValue(metadata.Identity, out (string Unevaluated, object Evaluated) value) ||
                        defaultValueByEditorType.TryGetValue(metadata.Editors.Last().TypeName, out value))
                    {
                        settingValue.UnevaluatedValue = value.Unevaluated;
                        settingValue.EvaluatedValue = value.Evaluated;
                    }

                    return new Setting(metadata, ImmutableArray.Create(settingValue));
                }
            }

            static Drawing FindDrawing(string resourceKey)
            {
                var resource = Application.Current.FindResource(resourceKey);
                Assumes.NotNull(resource);
                return (Drawing)resource;
            }
        }
    }
}