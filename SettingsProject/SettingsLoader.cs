using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace SettingsProject
{
    internal static class SettingsLoader
    {
        // TODO control 'Prefer 32-bit' visibility based on target framework(s)

        public static readonly ImmutableArray<SettingCondition> DefaultConditions = ImmutableArray.Create(
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

        public static readonly IImmutableDictionary<string, ImmutableArray<string>> DefaultConfigurationDictionary = new Dictionary<string, ImmutableArray<string>>
        {
            { "Configuration", ImmutableArray.Create("Debug", "Release") },
            { "Platform", ImmutableArray.Create("x86", "AnyCPU") },
            { "Target Framework", ImmutableArray.Create("net5.0", "net472") },
        }.ToImmutableDictionary();

        public static readonly SettingContext DefaultContext = new SettingContextBuilder(DefaultConfigurationDictionary, DefaultConditions, requireConditionMatches: true)
        {
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
                new SettingValue(ImmutableArray<string>.Empty, "ConsoleApp1")),
            new Setting(
                new SettingMetadata(
                    name: "Default namespace",
                    description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                    page: "Application",
                    category: "General",
                    priority: 200,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "ConsoleApp1")),
            new Setting(
                new SettingMetadata(
                    name: "Multi-targeting",
                    description: "Build this project for multiple target frameworks.",
                    page: "Application",
                    category: "General",
                    priority: 300,
                    editorType: "Bool"),
                new SettingValue(ImmutableArray<string>.Empty, false)),
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
                new SettingValue(ImmutableArray<string>.Empty, "net5")),
            new Setting(
                new SettingMetadata(
                    name: "Target framework",
                    description: "Specifies the framework that this project will target.",
                    page: "Application",
                    category: "General",
                    priority: 320,
                    editorType: "Enum"),
                new SettingValue(ImmutableArray<string>.Empty, ".NET 5")
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
                }),
            new Setting(
                new SettingMetadata(
                    name: "Install other frameworks",
                    description: null,
                    page: "Application",
                    category: "General",
                    priority: 400,
                    editorType: "LinkAction")
                {
                    EditorMetadata = new Dictionary<string, string>
                    {
                        { "Action", "URL" },
                        { "URL", "http://go.microsoft.com/fwlink/?LinkID=287120" }
                    }
                },
                values: ImmutableArray<SettingValue>.Empty),
            new Setting(
                new SettingMetadata(
                    name: "Output type",
                    description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                    page: "Application",
                    category: "General",
                    priority: 500,
                    editorType: "Enum"),
                new SettingValue(ImmutableArray<string>.Empty, "Console Application")
                {
                    EnumValues = ImmutableArray.Create(
                        "Console Application",
                        "Windows Application",
                        "Class Library")
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
                new SettingValue(ImmutableArray<string>.Empty, true)),
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
                value: new SettingValue(ImmutableArray<string>.Empty, "(Not set)")
                {
                    EnumValues = ImmutableArray.Create("(Not set)"),
                }),

            new Setting(
                new SettingMetadata(
                    name: "Resources",
                    description: "Specifies how application resources will be managed.",
                    page: "Application",
                    category: "Resources",
                    priority: 800,
                    editorType: "Enum"),
                value: new SettingValue(ImmutableArray<string>.Empty, "Icon and manifest")
                {
                    EnumValues = ImmutableArray.Create("Icon and manifest", "Resource file"),
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
                new SettingValue(ImmutableArray<string>.Empty, "(Default Icon)")),
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
                value: new SettingValue(ImmutableArray<string>.Empty, "")
                {
                    EnumValues = ImmutableArray.Create(""),
                }),
            new Setting(
                new SettingMetadata(
                    name: "Resource file path",
                    description: "Specifies a Win32 res file to compile into this project.",
                    page: "Application",
                    category: "Resources",
                    priority: 830,
                    editorType: "FileBrowse"),
                value: new SettingValue(ImmutableArray<string>.Empty, "")),

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
                value: new SettingValue(ImmutableArray<string>.Empty, "TRACE")),
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
                    new SettingValue(ImmutableArray.Create("Debug"), value: true),
                    new SettingValue(ImmutableArray.Create("Release"), value: false))),
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
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
                value: new SettingValue(ImmutableArray<string>.Empty, "Any CPU")
                {
                    EnumValues = ImmutableArray.Create("Any CPU", "x86")
                }),
            new Setting(
                new SettingMetadata(
                    name: "Nullable reference types",
                    description: "Controls use of nullable annotations and warnings.",
                    page: "Build",
                    category: "General",
                    priority: 500,
                    editorType: "Enum"),
                value: new SettingValue(ImmutableArray<string>.Empty, "Enable")
                {
                    EnumValues = ImmutableArray.Create("Disable", "Enable", "Warnings", "Annotations")
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
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
                    new SettingValue(ImmutableArray.Create("Debug"), value: false),
                    new SettingValue(ImmutableArray.Create("Release"), value: true))),

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
                value: new SettingValue(ImmutableArray<string>.Empty, "4")
                {
                    EnumValues = ImmutableArray.Create("0", "1", "2", "3", "4")
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
                new SettingValue(ImmutableArray<string>.Empty, "1701;1702")),
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
                value: new SettingValue(ImmutableArray<string>.Empty, "Specific warnings")
                {
                    EnumValues = ImmutableArray.Create("None", "All", "Specific warnings"),
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
                new SettingValue(ImmutableArray<string>.Empty, "NU1605")),

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
                new SettingValue(ImmutableArray<string>.Empty, "")),
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
                new SettingValue(ImmutableArray<string>.Empty, "")),
            // TODO this is disabled in .NET Core -- why?
            new Setting(
                new SettingMetadata(
                    name: "Register for COM interop",
                    description: "Add metadata from the output assembly to the registry, allowing COM clients to create .NET classes.",
                    page: "Build",
                    category: "Output",
                    priority: 300,
                    editorType: "Bool"),
                new SettingValue(ImmutableArray<string>.Empty, false)),
            new Setting(
                new SettingMetadata(
                    name: "Generate serialization assembly",
                    description: null,
                    page: "Build",
                    category: "Output",
                    priority: 400,
                    editorType: "Enum"),
                value: new SettingValue(ImmutableArray<string>.Empty, "Auto")
                {
                    EnumValues = ImmutableArray.Create("Auto", "On", "Off")
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
                    editorType: "LinkAction")
                {
                    EditorMetadata = new Dictionary<string, string>
                    {
                        { "Action", "URL" },
                        { "URL", "https://aka.ms/csharp-versions" }
                    }
                },
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
                value: new SettingValue(ImmutableArray<string>.Empty, "Prompt")
                {
                    EnumValues = ImmutableArray.Create("None", "Prompt", "Send", "Queue"),
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
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
                value: new SettingValue(ImmutableArray<string>.Empty, "Portable")
                {
                    EnumValues = ImmutableArray.Create("None", "Full", "Pdb-only", "Portable", "Embedded")
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
                value: new SettingValue(ImmutableArray<string>.Empty, "512")
                {
                    EnumValues = ImmutableArray.Create("512", "1024", "2048", "4096", "8192")
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
                new SettingValue(ImmutableArray<string>.Empty, "0x11000000")),

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
                    editorType: "MultiLineString")
                {
                    SupportsPerConfigurationValues = true
                },
                value: new SettingValue(ImmutableArray<string>.Empty, "")),
            new Setting(
                new SettingMetadata(
                    name: "Post-build event",
                    description: "Commands to execute after a build completes.",
                    page: "Build Events",
                    category: "General",
                    priority: 200,
                    editorType: "MultiLineString")
                {
                    SupportsPerConfigurationValues = true
                },
                value: new SettingValue(ImmutableArray<string>.Empty, "")),
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
                value: new SettingValue(ImmutableArray<string>.Empty, "On successful build")
                {
                    EnumValues = ImmutableArray.Create(
                        "Always",
                        "On successful build",
                        "When the build updates the project output")
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
            new Setting(
                new SettingMetadata(
                    name: "Package ID",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 300,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "ConsoleApp1")),
            // TODO VersionSetting (note -- has different validation rules to assembly/file versions)
            new Setting(
                new SettingMetadata(
                    name: "Package version",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 400,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "1.0.0")),
            new Setting(
                new SettingMetadata(
                    name: "Authors",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 500,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "ConsoleApp1")),
            new Setting(
                new SettingMetadata(
                    name: "Company",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 600,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "ConsoleApp1")),
            new Setting(
                new SettingMetadata(
                    name: "Product",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 700,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "ConsoleApp1")),
            new Setting(
                new SettingMetadata(
                    name: "Description",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 800,
                    editorType: "MultiLineString"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            new Setting(
                new SettingMetadata(
                    name: "Copyright",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 900,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            // TODO make this IconBrowseSetting
            new Setting(
                new SettingMetadata(
                    name: "Package icon file",
                    description: "Path to the icon to include in and use for the package.",
                    page: "Packaging",
                    category: "General",
                    priority: 1100,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            new Setting(
                new SettingMetadata(
                    name: "Repository URL",
                    description: null, // TODO describe what this URL means
                    page: "Packaging",
                    category: "General",
                    priority: 1200,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            // TODO provide feedback about valid URLs here
            new Setting(
                new SettingMetadata(
                    name: "Repository type",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1300,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            new Setting(
                new SettingMetadata(
                    name: "Tags",
                    description: null, // TODO describe how this is delimited
                    page: "Packaging",
                    category: "General",
                    priority: 1400,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            new Setting(
                new SettingMetadata(
                    name: "Release notes",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1500,
                    editorType: "MultiLineString"),
                new SettingValue(ImmutableArray<string>.Empty, "")),
            // TODO this is a combo box with many languages listed
            new Setting(
                new SettingMetadata(
                    name: "Assembly neutral language",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1600,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "(None)")),
            // TODO VersionSetting
            new Setting(
                new SettingMetadata(
                    name: "Assembly version",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1700,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "1.0.0.0")),
            // TODO VersionSetting
            new Setting(
                new SettingMetadata(
                    name: "Assembly file version",
                    description: null,
                    page: "Packaging",
                    category: "General",
                    priority: 1800,
                    editorType: "String"),
                new SettingValue(ImmutableArray<string>.Empty, "1.0.0.0")),

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
                new SettingValue(ImmutableArray<string>.Empty, false)),
            new Setting(
                new SettingMetadata(
                    name: "License specification",
                    description: "Controls how the package's license is specified.",
                    page: "Packaging",
                    category: "License",
                    priority: 200,
                    editorType: "Enum"),
                value: new SettingValue(ImmutableArray<string>.Empty, "None")
                {
                    EnumValues = ImmutableArray.Create("None", "Expression", "File")
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
                new SettingValue(ImmutableArray<string>.Empty, "")),
            new Setting(
                new SettingMetadata(
                    name: "Read about SPDX license expressions",
                    description: null,
                    page: "Packaging",
                    category: "License",
                    priority: 400,
                    editorType: "LinkAction")
                {
                    EditorMetadata = new Dictionary<string, string>
                    {
                        { "Action", "URL" },
                        { "URL", "https://spdx.org/licenses/" }
                    }
                },
                values: ImmutableArray<SettingValue>.Empty),
            new Setting(
                new SettingMetadata(
                    name: "License file path",
                    description: "The path to the license file to include in the package. May be relative to the project directory.",
                    page: "Packaging",
                    category: "License",
                    priority: 500,
                    editorType: "FileBrowse"),
                new SettingValue(ImmutableArray<string>.Empty, "")),

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
                    editorType: "LinkAction")
                {
                    EditorMetadata = new Dictionary<string, string>
                    {
                        { "Action", "Command" },
                        { "Command", "ManageLaunchProfiles" }
                    }
                },
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
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
                new SettingValue(ImmutableArray<string>.Empty, "")),
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
                new SettingValue(ImmutableArray<string>.Empty, false)),

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
                    editorType: "LinkAction")
                {
                    EditorMetadata = new Dictionary<string, string>
                    {
                        { "Action", "URL" },
                        { "URL", "https://docs.microsoft.com/visualstudio/code-quality/roslyn-analyzers-overview" }
                    }
                },
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
                new SettingValue(ImmutableArray<string>.Empty, false)),
            new Setting(
                new SettingMetadata(
                    name: "Run live analysis",
                    description: "Run analyzers live in the IDE.",
                    page: "Code Analysis",
                    category: "Analyzers",
                    priority: 300,
                    editorType: "Bool"),
                new SettingValue(ImmutableArray<string>.Empty, false))
        }.Build();
    }
}