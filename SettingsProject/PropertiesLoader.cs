using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal static class PropertiesLoader
    {
        private static readonly ImmutableDictionary<string, string> DebugConfiguration = new Dictionary<string, string> {{"Configuration", "Debug"}}.ToImmutableDictionary(StringComparers.ConfigurationDimensionNames);
        private static readonly ImmutableDictionary<string, string> ReleaseConfiguration = new Dictionary<string, string> {{"Configuration", "Release"}}.ToImmutableDictionary(StringComparers.ConfigurationDimensionNames);

        // TODO control 'Prefer 32-bit' visibility based on target framework(s)

        private static readonly ImmutableArray<PropertyCondition> DefaultConditions = ImmutableArray.Create(
            // Multi-targeting
            new PropertyCondition(
                source: new PropertyIdentity("Application", "General", "Multi-targeting"),
                sourceValue: true,
                target: new PropertyIdentity("Application", "General", "Target frameworks")),
            new PropertyCondition(
                source: new PropertyIdentity("Application", "General", "Multi-targeting"),
                sourceValue: false,
                target: new PropertyIdentity("Application", "General", "Target framework")),
            // Resources
            new PropertyCondition(
                source: new PropertyIdentity("Application", "Resources", "Resources"),
                sourceValue: "Icon and manifest",
                target: new PropertyIdentity("Application", "Resources", "Icon path")),
            new PropertyCondition(
                source: new PropertyIdentity("Application", "Resources", "Resources"),
                sourceValue: "Icon and manifest",
                target: new PropertyIdentity("Application", "Resources", "Manifest path")),
            new PropertyCondition(
                source: new PropertyIdentity("Application", "Resources", "Resources"),
                sourceValue: "Resource file",
                target: new PropertyIdentity("Application", "Resources", "Resource file path")),

            new PropertyCondition(
                source: new PropertyIdentity("Packaging", "License", "License specification"),
                sourceValue: "Expression",
                target: new PropertyIdentity("Packaging", "License", "License expression")),
            new PropertyCondition(
                source: new PropertyIdentity("Packaging", "License", "License specification"),
                sourceValue: "Expression",
                target: new PropertyIdentity("Packaging", "License", "Read about SPDX license expressions")),
            new PropertyCondition(
                source: new PropertyIdentity("Packaging", "License", "License specification"),
                sourceValue: "File",
                target: new PropertyIdentity("Packaging", "License", "License file path")),

            new PropertyCondition(
                source: new PropertyIdentity("Signing", "General", "Signing"),
                sourceValue: true,
                target: new PropertyIdentity("Signing", "General", "Key file path")),
            new PropertyCondition(
                source: new PropertyIdentity("Signing", "General", "Signing"),
                sourceValue: true,
                target: new PropertyIdentity("Signing", "General", "Delay signing"))
        );

        public static PropertyContext CreateDefaultContext() => new PropertyContext(
            new KeyValuePair<string, ImmutableArray<string>>[]
            {
                new ("Configuration", ImmutableArray.Create("Debug", "Release")),
                new ("Platform", ImmutableArray.Create("x86", "AnyCPU")),
                new ("Target Framework", ImmutableArray.Create("net5.0", "net472"))
            },
            DefaultConditions,
            ImmutableArray.Create(

                /////////////
                //////////// APPLICATION
                ///////////

                //////
                ///// GENERAL
                ////

                new Property(
                    new PropertyMetadata(
                        name: "Assembly name",
                        description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
                        page: "Application",
                        category: "General",
                        priority: 10,
                        editorType: "String"),
                    new PropertyValue("$(ProjectName)", "ConsoleApp1")),
                new Property(
                    new PropertyMetadata(
                        name: "Default namespace",
                        description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                        page: "Application",
                        category: "General",
                        priority: 200,
                        editorType: "String"),
                    new PropertyValue("$(DefaultNamespace)", "ConsoleApp1")),
                new Property(
                    new PropertyMetadata(
                        name: "Multi-targeting",
                        description: "Build this project for multiple target frameworks.",
                        page: "Application",
                        category: "General",
                        priority: 300,
                        editorType: "Bool"),
                    new PropertyValue("false", false)),
                // TODO come up with a better editing experience, perhaps via a FlagsProperty
                // TODO allow completion of values: new[] { ".net5", ".netcoreapp3.1", ".netcoreapp3.0", ".netcoreapp2.2", ".netcoreapp2.1", ".netcoreapp2.0", ".netcoreapp1.1", ".netcoreapp1.0" }
                new Property(
                    new PropertyMetadata(
                        name: "Target frameworks",
                        description: "Specifies the semicolon-delimited list of frameworks that this project will target.",
                        page: "Application",
                        category: "General",
                        priority: 310,
                        editorType: "String"),
                    new PropertyValue("net5", "net5")),
                new Property(
                    new PropertyMetadata(
                        name: "Target framework",
                        description: "Specifies the framework that this project will target.",
                        page: "Application",
                        category: "General",
                        priority: 320,
                        editorType: "Enum"),
                    new PropertyValue("net5.0", "net5.0")
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
                new Property(
                    new PropertyMetadata(
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
                    values: ImmutableArray<PropertyValue>.Empty),
                new Property(
                    new PropertyMetadata(
                        name: "Output type",
                        description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                        page: "Application",
                        category: "General",
                        priority: 500,
                        editorType: "Enum"),
                    new PropertyValue("Exe", "Exe")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Console Application", "Exe"),
                            new SupportedValue("Windows Application", "WinExe"),
                            new SupportedValue("Class Library", ""))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Binding redirects",
                        description: "Whether to auto-generate binding redirects.",
                        page: "Application",
                        category: "General",
                        priority: 600,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("true", true)),
                new Property(
                    new PropertyMetadata(
                        name: "Startup object",
                        description: "Specifies the entry point for the executable.",
                        page: "Application",
                        category: "General",
                        priority: 700,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("(Not set)", "(Not set)")
                    {
                        SupportedValues = ImmutableArray.Create(new SupportedValue("(Not set)")),
                    }),

                new Property(
                    new PropertyMetadata(
                        name: "Resources",
                        description: "Specifies how application resources will be managed.",
                        page: "Application",
                        category: "Resources",
                        priority: 800,
                        editorType: "Enum"),
                    value: new PropertyValue("Icon and manifest", "Icon and manifest")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Icon and manifest"),
                            new SupportedValue("Resource file")),
                    }),
                // TODO make this IconBrowseProperty
                new Property(
                    new PropertyMetadata(
                        name: "Icon path",
                        description: "Path to the icon to embed into the output assembly.",
                        page: "Application",
                        category: "Resources",
                        priority: 810,
                        editorType: "String"),
                    new PropertyValue("(Default Icon)", "(Default Icon)")),
                // TODO make this FileBrowseProperty
                // TODO this can appear disabled, find out why
                new Property(
                    new PropertyMetadata(
                        name: "Manifest path",
                        description: "A manifest determines specific settings for an application. To embed a custom manifest, first add it to your project and then select it from the list.",
                        page: "Application",
                        category: "Resources",
                        priority: 820,
                        editorType: "Enum"),
                    value: new PropertyValue("", "")
                    {
                        SupportedValues = ImmutableArray.Create(new SupportedValue(""))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Resource file path",
                        description: "Specifies a Win32 res file to compile into this project.",
                        page: "Application",
                        category: "Resources",
                        priority: 830,
                        editorType: "FileBrowse"),
                    value: new PropertyValue("", "")),

                //////
                ///// ASSEMBLY INFORMATION
                ////

                // TODO this section is disabled for .NET Core projects -- if we have time, determine whether there's anything in here we couldn't add later

//                new Property(
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

                new Property(
                    new PropertyMetadata(
                        name: "Conditional compilation symbols",
                        description: "A semicolon-delimited list of symbols to define for the compilation.",
                        page: "Build",
                        category: "General",
                        priority: 30,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("TRACE", "TRACE")),
                new Property(
                    new PropertyMetadata(
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
                        new PropertyValue("true", evaluatedValue: true, configurationDimensions: DebugConfiguration),
                        new PropertyValue("false", evaluatedValue: false, configurationDimensions: ReleaseConfiguration))),
                new Property(
                    new PropertyMetadata(
                        name: "Define TRACE symbol",
                        description: "Specifies whether to define the TRACE compilation symbol.",
                        page: "Build",
                        category: "General",
                        priority: 300,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "Platform target",
                        description: "The platform to target in this project configuration.",
                        page: "Build",
                        category: "General",
                        priority: 400,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("Any CPU", "Any CPU")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Any CPU"),
                            new SupportedValue("x86"))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Nullable reference types",
                        description: "Controls use of nullable annotations and warnings.",
                        page: "Build",
                        category: "General",
                        priority: 500,
                        editorType: "Enum"),
                    value: new PropertyValue("enable", "Enable")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Disable"),
                            new SupportedValue("Enable"),
                            new SupportedValue("Warnings"),
                            new SupportedValue("Annotations"))
                    }),
                // TODO this is disabled in .NET Core -- why?
                new Property(
                    new PropertyMetadata(
                        name: "Prefer 32-bit",
                        description: "Specifies whether to prefer 32-bit when available.",
                        page: "Build",
                        category: "General",
                        priority: 600,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "Unsafe code",
                        description: "Allow unsafe code in this project.",
                        page: "Build",
                        category: "General",
                        priority: 700,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
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
                        new PropertyValue("false", evaluatedValue: false, configurationDimensions: DebugConfiguration),
                        new PropertyValue("true", evaluatedValue: true, configurationDimensions: ReleaseConfiguration))),

                //////
                ///// ERRORS AND WARNINGS
                ////

                new Property(
                    new PropertyMetadata(
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
                    value: new PropertyValue("4", "4")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("0"),
                            new SupportedValue("1"),
                            new SupportedValue("2"),
                            new SupportedValue("3"),
                            new SupportedValue("4"))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Suppress specific warnings",
                        description: "A semicolon-delimited list of warning codes to suppress.",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 200,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("1701;1702", "1701;1702")),
                new Property(
                    new PropertyMetadata(
                        name: "Warnings as errors",
                        description: "Controls which warnings are treated as errors.",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 300,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("Specific warnings", "Specific warnings")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("All"),
                            new SupportedValue("Specific warnings"))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Treat specific warnings as errors",
                        description: "A semicolon-delimited list of warning codes to treat as errors.",
                        page: "Build",
                        category: "Errors and warnings",
                        priority: 400,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("NU1605", "NU1605")),

                //////
                ///// OUTPUT
                ////

                new Property(
                    new PropertyMetadata(
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
                        new PropertyValue("bin\\$(Configuration)", "bin\\Debug", DebugConfiguration),
                        new PropertyValue("bin\\$(Configuration)", "bin\\Release", ReleaseConfiguration))),
                new Property(
                    new PropertyMetadata(
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
                        new PropertyValue("$(OutputPath)\\$(ProjectName).xml", "bin\\Debug\\ConsoleApp1.xml", DebugConfiguration),
                        new PropertyValue("$(OutputPath)\\$(ProjectName).xml", "bin\\Release\\ConsoleApp1.xml", ReleaseConfiguration))),
                // TODO this is disabled in .NET Core -- why?
                new Property(
                    new PropertyMetadata(
                        name: "Register for COM interop",
                        description: "Add metadata from the output assembly to the registry, allowing COM clients to create .NET classes.",
                        page: "Build",
                        category: "Output",
                        priority: 300,
                        editorType: "Bool"),
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "Generate serialization assembly",
                        description: null,
                        page: "Build",
                        category: "Output",
                        priority: 400,
                        editorType: "Enum"),
                    value: new PropertyValue("Auto", "Auto")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("Auto"),
                            new SupportedValue("On"),
                            new SupportedValue("Off"))
                    }),

                //////
                ///// ADVANCED
                ////

                new Property(
                    new PropertyMetadata(
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
                    values: ImmutableArray<PropertyValue>.Empty),

                new Property(
                    new PropertyMetadata(
                        name: "Internal compiler error reporting",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 200,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("Prompt", "Prompt")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("Prompt"),
                            new SupportedValue("Send"),
                            new SupportedValue("Queue"))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Overflow checking",
                        description: "Enable arithmetic overflow checking at runtime.",
                        page: "Build",
                        category: "Advanced",
                        priority: 300,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "Debugging information",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 400,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("Portable", "Portable")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("Full"),
                            new SupportedValue("Pdb-only"),
                            new SupportedValue("Portable"),
                            new SupportedValue("Embedded"))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "File alignment",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 500,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("512", "512")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("512"),
                            new SupportedValue("1024"),
                            new SupportedValue("2048"),
                            new SupportedValue("4096"),
                            new SupportedValue("8192"))
                    }),
                new Property(
                    new PropertyMetadata(
                        name: "Library base address",
                        description: null,
                        page: "Build",
                        category: "Advanced",
                        priority: 600,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("0x11000000", "0x11000000")),

                /////////////
                //////////// BUILD EVENTS
                ///////////

                //////
                ///// GENERAL
                ////

                // TODO both these build events can be edited in a pop-out editor with macro support
                new Property(
                    new PropertyMetadata(
                        name: "Pre-build event",
                        description: "Commands to execute before a build occurs.",
                        page: "Build Events",
                        category: "General",
                        priority: 70,
                        editorType: "MultiLineString"),
                    value: new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Post-build event",
                        description: "Commands to execute after a build completes.",
                        page: "Build Events",
                        category: "General",
                        priority: 200,
                        editorType: "MultiLineString"),
                    value: new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Run the post-build event",
                        description: "Controls when any post-build event is executed.",
                        page: "Build Events",
                        category: "General",
                        priority: 300,
                        editorType: "Enum")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    value: new PropertyValue("On successful build", "On successful build")
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

                new Property(
                    new PropertyMetadata(
                        name: "Generate NuGet package on build",
                        description: "Specifies whether a NuGet package should be produced in the output directory when the project is build.",
                        page: "Packaging",
                        category: "General",
                        priority: 80,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "Package ID",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 300,
                        editorType: "String"),
                    new PropertyValue("$(ProjectName)", "ConsoleApp1")),
                // TODO VersionProperty (note -- has different validation rules to assembly/file versions)
                new Property(
                    new PropertyMetadata(
                        name: "Package version",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 400,
                        editorType: "String"),
                    new PropertyValue("$(Version)", "1.0.0")),
                new Property(
                    new PropertyMetadata(
                        name: "Authors",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 500,
                        editorType: "String"),
                    new PropertyValue("$(ProjectName)", "ConsoleApp1")),
                new Property(
                    new PropertyMetadata(
                        name: "Company",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 600,
                        editorType: "String"),
                    new PropertyValue("$(ProjectName)", "ConsoleApp1")),
                new Property(
                    new PropertyMetadata(
                        name: "Product",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 700,
                        editorType: "String"),
                    new PropertyValue("$(ProjectName)", "ConsoleApp1")),
                new Property(
                    new PropertyMetadata(
                        name: "Description",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 800,
                        editorType: "MultiLineString"),
                    new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Copyright",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 900,
                        editorType: "String"),
                    new PropertyValue("", "")),
                // TODO make this IconBrowseProperty
                new Property(
                    new PropertyMetadata(
                        name: "Package icon file",
                        description: "Path to the icon to include in and use for the package.",
                        page: "Packaging",
                        category: "General",
                        priority: 1100,
                        editorType: "String"),
                    new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Repository URL",
                        description: null, // TODO describe what this URL means
                        page: "Packaging",
                        category: "General",
                        priority: 1200,
                        editorType: "String"),
                    new PropertyValue("", "")),
                // TODO provide feedback about valid URLs here
                new Property(
                    new PropertyMetadata(
                        name: "Repository type",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1300,
                        editorType: "String"),
                    new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Tags",
                        description: null, // TODO describe how this is delimited
                        page: "Packaging",
                        category: "General",
                        priority: 1400,
                        editorType: "String"),
                    new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Release notes",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1500,
                        editorType: "MultiLineString"),
                    new PropertyValue("", "")),
                // TODO this is a combo box with many languages listed
                new Property(
                    new PropertyMetadata(
                        name: "Assembly neutral language",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1600,
                        editorType: "String"),
                    new PropertyValue("(None)", "(None)")),
                // TODO VersionProperty
                new Property(
                    new PropertyMetadata(
                        name: "Assembly version",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1700,
                        editorType: "String"),
                    new PropertyValue("1.0.0.0", "1.0.0.0")),
                // TODO VersionProperty
                new Property(
                    new PropertyMetadata(
                        name: "Assembly file version",
                        description: null,
                        page: "Packaging",
                        category: "General",
                        priority: 1800,
                        editorType: "String"),
                    new PropertyValue("1.0.0.0", "1.0.0.0")),

                //////
                ///// LICENSE
                ////

                new Property(
                    new PropertyMetadata(
                        name: "Require license acceptance",
                        description: "Controls whether consumers of the generated package are presented with a license acceptance prompt when adding a reference to this package.",
                        page: "Packaging",
                        category: "License",
                        priority: 85,
                        editorType: "Bool"),
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "License specification",
                        description: "Controls how the package's license is specified.",
                        page: "Packaging",
                        category: "License",
                        priority: 200,
                        editorType: "Enum"),
                    value: new PropertyValue("None", "None")
                    {
                        SupportedValues = ImmutableArray.Create(
                            new SupportedValue("None"),
                            new SupportedValue("Expression"),
                            new SupportedValue("File"))
                    }),
                // TODO provide some examples for auto-complete: Apache-2.0;MIT;...
                new Property(
                    new PropertyMetadata(
                        name: "License expression",
                        description: "The SPDX expression that specifies the package's license.",
                        page: "Packaging",
                        category: "License",
                        priority: 300,
                        editorType: "String"),
                    new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
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
                    values: ImmutableArray<PropertyValue>.Empty),
                new Property(
                    new PropertyMetadata(
                        name: "License file path",
                        description: "The path to the license file to include in the package. May be relative to the project directory.",
                        page: "Packaging",
                        category: "License",
                        priority: 500,
                        editorType: "FileBrowse"),
                    new PropertyValue("", "")),

                /////////////
                //////////// DEBUG
                ///////////

                //////
                ///// GENERAL
                ////

                // TODO make this link action show the launch profiles UI
                new Property(
                    new PropertyMetadata(
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
                    values: ImmutableArray<PropertyValue>.Empty),

                /////////////
                //////////// SIGNING
                ///////////

                //////
                ///// GENERAL
                ////

                new Property(
                    new PropertyMetadata(
                        name: "Signing",
                        description: "Sign the project's output assembly.",
                        page: "Signing",
                        category: "General",
                        priority: 92,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                // TODO StrongNameKeyProperty -- with new/add and change password actions
                new Property(
                    new PropertyMetadata(
                        name: "Key file path",
                        description: "Choose a string name key file",
                        page: "Signing",
                        category: "General",
                        priority: 110,
                        editorType: "String")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("", "")),
                new Property(
                    new PropertyMetadata(
                        name: "Delay signing",
                        description: "Delay sign the assembly. When enabled the project will not run or be debuggable.",
                        page: "Signing",
                        category: "General",
                        priority: 120,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),

                /////////////
                //////////// CODE ANALYSIS
                ///////////

                //////
                ///// ANALYZERS
                ////

                new Property(
                    new PropertyMetadata(
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
                    values: ImmutableArray<PropertyValue>.Empty),
                new Property(
                    new PropertyMetadata(
                        name: "Run on build",
                        description: "Run analyzers during build.",
                        page: "Code Analysis",
                        category: "Analyzers",
                        priority: 200,
                        editorType: "Bool")
                    {
                        SupportsPerConfigurationValues = true
                    },
                    new PropertyValue("false", false)),
                new Property(
                    new PropertyMetadata(
                        name: "Run live analysis",
                        description: "Run analyzers live in the IDE.",
                        page: "Code Analysis",
                        category: "Analyzers",
                        priority: 300,
                        editorType: "Bool"),
                    new PropertyValue("false", false))
            ));

        #region PropertyMetadata

        private static readonly PropertyMetadata ExecutablePath = new PropertyMetadata(
            name: "Executable path",
            description: "Path to the executable to debug.",
            page: "Debug",
            category: "General",
            priority: 100,
            editorType: "FileBrowse");

        private static readonly PropertyMetadata ApplicationArguments = new PropertyMetadata(
            name: "Application arguments",
            description: "Arguments to be passed to the launched application.",
            page: "Debug",
            category: "General",
            priority: 200,
            editorType: "String");

        private static readonly PropertyMetadata WorkingDirectory = new PropertyMetadata(
            name: "Working directory",
            description: "Absolute path to the working directory.",
            page: "Debug",
            category: "General",
            priority: 300,
            editorType: "FileBrowse");

        private static readonly PropertyMetadata UseRemoteMachine = new PropertyMetadata(
            name: "Use remote machine",
            description: "The debug target is on a remote machine.",
            page: "Debug",
            category: "General",
            priority: 400,
            editorType: "Bool");

        private static readonly PropertyMetadata RemoteMachineHostName = new PropertyMetadata(
            name: "Remote machine host name",
            description: null,
            page: "Debug",
            category: "General",
            priority: 410,
            editorType: "String"); // TODO RemoteMachineProperty, with support for the 'Find' button

        private static readonly PropertyMetadata AuthenticationMode = new PropertyMetadata(
            name: "Authentication mode",
            description: null,
            page: "Debug",
            category: "General",
            priority: 420,
            editorType: "Enum");

        private static readonly PropertyMetadata LaunchBrowser = new PropertyMetadata(
            name: "Launch Browser",
            description: "Whether a browser should be launched when this profile is invoked.",
            page: "Debug",
            category: "General",
            priority: 550,
            editorType: "Bool");

        private static readonly PropertyMetadata LaunchBrowserUrl = new PropertyMetadata(
            name: "Launch Browser URL",
            description: "Absolute or relative URL to direct the browser to when launched.",
            page: "Debug",
            category: "General",
            priority: 560,
            editorType: "String");

        private static readonly PropertyMetadata EnvironmentVariables = new PropertyMetadata(
            name: "Environment variables",
            description: "Specifies environment variables to be set for the launched application.",
            page: "Debug",
            category: "General",
            priority: 500,
            editorType: "String"); // TODO NameValueList

        private static readonly PropertyMetadata NativeCodeDebugging = new PropertyMetadata(
            name: "Native code debugging",
            description: "Enable native code debugging.",
            page: "Debug",
            category: "General",
            priority: 600,
            editorType: "Bool");

        private static readonly PropertyMetadata SqlServerDebugging = new PropertyMetadata(
            name: "SQL Server debugging",
            description: "Enable SQL Server debugging.",
            page: "Debug",
            category: "General",
            priority: 700,
            editorType: "Bool");

        private static readonly PropertyMetadata AzureResource = new PropertyMetadata(
            name: "Azure resource",
            description: "The Azure resource to use in your snapshot debugging session.",
            page: "Debug",
            category: "General",
            priority: 100,
            editorType: "Enum"); // TODO AzureResource

        private static readonly PropertyMetadata AzureStorageAccount = new PropertyMetadata(
            name: "Azure Storage account",
            description: "The Azure resource to use in your snapshot debugging session.",
            page: "Debug",
            category: "General",
            priority: 200,
            editorType: "Enum"); // TODO editorType AzureStorage

        private static readonly PropertyMetadata AppUrl = new PropertyMetadata(
            name: "App URL",
            description: "The URL at which the application will be hosted when running.",
            page: "Debug",
            category: "Web Server Settings",
            priority: 1100,
            editorType: "String");

        private static readonly PropertyMetadata IisExpressBitness = new PropertyMetadata(
            name: "IIS Express Bitness",
            description: "Bitness of the IIS Express process to launch (x86, x64).",
            page: "Debug",
            category: "Web Server Settings",
            priority: 1200,
            editorType: "Enum");

        private static readonly PropertyMetadata HostingModel = new PropertyMetadata(
            name: "Hosting Model",
            description: "The URL at which the application will be hosted when running.",
            page: "Debug",
            category: "Web Server Settings",
            priority: 1300,
            editorType: "Enum");

        private static readonly PropertyMetadata EnableSSL = new PropertyMetadata(
            name: "Enable SSL",
            description: null,
            page: "Debug",
            category: "Web Server Settings",
            priority: 1400,
            editorType: "Bool");

        private static readonly PropertyMetadata EnableAnonymousAuthentication = new PropertyMetadata(
            name: "Enable Anonymous Authentication",
            description: null,
            page: "Debug",
            category: "Web Server Settings",
            priority: 1500,
            editorType: "Bool");

        private static readonly PropertyMetadata EnableWindowsAuthentication = new PropertyMetadata(
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
                new PropertyCondition(
                    source: UseRemoteMachine.Identity,
                    sourceValue: true,
                    target: RemoteMachineHostName.Identity),
                new PropertyCondition(
                    source: UseRemoteMachine.Identity,
                    sourceValue: true,
                    target: AuthenticationMode.Identity));

            var launchBrowserConditions = ImmutableArray.Create(
                new PropertyCondition(
                    source: LaunchBrowser.Identity,
                    sourceValue: true,
                    target: LaunchBrowserUrl.Identity));

            var executableKindPropertyMetadata = ImmutableArray.Create(
                ExecutablePath,
                ApplicationArguments,
                WorkingDirectory,
                UseRemoteMachine,
                RemoteMachineHostName,
                AuthenticationMode,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging);

            var projectKindPropertyMetadata = ImmutableArray.Create(
                ApplicationArguments,
                WorkingDirectory,
                UseRemoteMachine,
                RemoteMachineHostName,
                AuthenticationMode,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging);

            var snapshotDebuggerKindPropertyMetadata = ImmutableArray.Create(
                AzureResource,
                AzureStorageAccount);

            var iisExpressKindPropertyMetadata = ImmutableArray.Create(
                ApplicationArguments,
                WorkingDirectory,
                LaunchBrowser,
                LaunchBrowserUrl,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging,
                // Web server properties
                AppUrl,
                IisExpressBitness,
                HostingModel,
                EnableSSL,
                EnableAnonymousAuthentication,
                EnableWindowsAuthentication);

            var iisKindPropertyMetadata = ImmutableArray.Create(
                ApplicationArguments,
                WorkingDirectory,
                LaunchBrowser,
                LaunchBrowserUrl,
                EnvironmentVariables,
                NativeCodeDebugging,
                SqlServerDebugging,
                // Web server properties
                AppUrl,
                IisExpressBitness,
                HostingModel,
                EnableSSL,
                EnableAnonymousAuthentication,
                EnableWindowsAuthentication);

            var projectKind = new LaunchProfileKind("Project", projectKindPropertyMetadata, remoteMachineConditions, FindDrawing("IconApplicationDrawing"));
            var executableKind = new LaunchProfileKind("Executable", executableKindPropertyMetadata, remoteMachineConditions, FindDrawing("IconExecuteDrawing"));
            var snapshotDebuggerKind = new LaunchProfileKind("Snapshot Debugger", snapshotDebuggerKindPropertyMetadata, ImmutableArray<PropertyCondition>.Empty, FindDrawing("SnapshotDebuggerDrawing"));
            var iisKind = new LaunchProfileKind("IIS", iisKindPropertyMetadata, launchBrowserConditions, FindDrawing("IISDrawing"));
            var iisExpressKind = new LaunchProfileKind("IIS Express", iisExpressKindPropertyMetadata, launchBrowserConditions, FindDrawing("IISExpressDrawing"));

            var supportedValuesByProperty = new Dictionary<PropertyIdentity, ImmutableArray<SupportedValue>>
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
                CreateLaunchProfileViewModel("My project", projectKind, new Dictionary<PropertyIdentity, (string Unevaluated, object Evaluated)>
                {
                    { ApplicationArguments.Identity, ("/foo /bar", "/foo /bar") }
                }),
                CreateLaunchProfileViewModel("devenv.exe", executableKind, new Dictionary<PropertyIdentity, (string Unevaluated, object Evaluated)>
                {
                    { ExecutablePath.Identity, ("devenv.exe", "devenv.exe") },
                    { ApplicationArguments.Identity, ("/rootSuffix Exp", "/rootSuffix Exp") }
                }),
                CreateLaunchProfileViewModel("My Snapshot", snapshotDebuggerKind, new Dictionary<PropertyIdentity, (string Unevaluated, object Evaluated)>
                {
                    // TODO
                }),
                CreateLaunchProfileViewModel("My IIS", iisKind, new Dictionary<PropertyIdentity, (string Unevaluated, object Evaluated)>
                {
                    { AppUrl.Identity, ("http://localhost:52531", "http://localhost:52531") },
                    { LaunchBrowser.Identity, ("true", true) }
                }),
                CreateLaunchProfileViewModel("My IIS Express", iisExpressKind, new Dictionary<PropertyIdentity, (string Unevaluated, object Evaluated)>
                {
                    { AppUrl.Identity, ("http://localhost:52531", "http://localhost:52531") },
                    { LaunchBrowser.Identity, ("true", true) }
                })
            };

            return new LaunchProfilesWindowViewModel(profiles, profileKinds);

            LaunchProfileViewModel CreateLaunchProfileViewModel(string name, LaunchProfileKind kind, Dictionary<PropertyIdentity, (string Unevaluated, object Evaluated)> initialValues)
            {
                var context = new PropertyContext(
                    Array.Empty<KeyValuePair<string, ImmutableArray<string>>>(),
                    kind.Conditions,
                    kind.Metadata.Select(CreateProperty).ToImmutableArray());

                return new LaunchProfileViewModel(name, kind, context);

                Property CreateProperty(PropertyMetadata metadata)
                {
                    // Debug launch profile values are unconfigured
                    var propertyValue = new PropertyValue(unevaluatedValue: "", evaluatedValue: "");

                    if (supportedValuesByProperty.TryGetValue(metadata.Identity, out ImmutableArray<SupportedValue> supportedValues))
                    {
                        propertyValue.SupportedValues = supportedValues;
                        propertyValue.EvaluatedValue = supportedValues.First();
                    }

                    if (initialValues.TryGetValue(metadata.Identity, out (string Unevaluated, object Evaluated) value) ||
                        defaultValueByEditorType.TryGetValue(metadata.Editors.Last().TypeName, out value))
                    {
                        propertyValue.UnevaluatedValue = value.Unevaluated;
                        propertyValue.EvaluatedValue = value.Evaluated;
                    }

                    return new Property(metadata, ImmutableArray.Create(propertyValue));
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