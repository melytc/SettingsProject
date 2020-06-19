using System;
using System.Collections.Generic;

#nullable enable

namespace SettingsProject
{
    internal static class SettingsLoader
    {
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
                initialValue: "ConsoleApp1",
                priority: 100,
                defaultValue: "ConsoleApp1",
                description: "Specifies the name of the generated assembly, both on the file system and in metadata.",
                page: "Application",
                category: "General"),
            new StringSetting(
                name: "Default namespace",
                initialValue: "ConsoleApp1",
                priority: 200,
                defaultValue: "ConsoleApp1",
                description: "Specifies the root namespace for the project, which controls code generation and analyzers.",
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Multi-targeting",
                initialValue: false,
                defaultValue: null,
                description: "Build this project for multiple target frameworks.",
                priority: 300,
                page: "Application",
                category: "General",
                trueSettings: new Setting[]
                {
                    // TODO come up with a better editing experience, perhaps via a FlagsEnumSetting
                    // TODO allow completion of values: new[] { ".net5", ".netcoreapp3.1", ".netcoreapp3.0", ".netcoreapp2.2", ".netcoreapp2.1", ".netcoreapp2.0", ".netcoreapp1.1", ".netcoreapp1.0" }
                    new StringSetting(
                        name: "Target frameworks",
                        initialValue: "net5",
                        priority: 100,
                        defaultValue: null,
                        description: "Specifies the semicolon-delimited list of frameworks that this project will target.",
                        page: "Application",
                        category: "General"),
                },
                falseSettings: new Setting[]
                {
                    new EnumSetting(
                        name: "Target framework",
                        initialValue: ".NET 5",
                        defaultValue: null,
                        priority: 100,
                        description: "Specifies the framework that this project will target.",
                        page: "Application",
                        category: "General",
                        enumValues: new[] { ".NET 5", ".NET Core 3.1", ".NET Core 3.0", ".NET Core 2.2", ".NET Core 2.1", ".NET Core 2.0", ".NET Core 1.1", ".NET Core 1.0" }),
                }),
            new LinkAction(
                name: "Install other frameworks",
                priority: 400,
                page: "Application",
                category: "General"),
            new EnumSetting(
                name: "Output type",
                initialValue: "Console Application",
                defaultValue: null,
                enumValues: new[] { "Console Application", "Windows Application", "Class Library" },
                priority: 500,
                description: "Specifies whether the output is executable, and whether it runs in a console or as a desktop application.",
                page: "Application",
                category: "General"),
            new BoolSetting(
                name: "Binding redirects",
                initialValue: true,
                defaultValue: true,
                description: "Whether to auto-generate binding redirects.",
                priority: 600,
                page: "Application",
                category: "General"),
            new EnumSetting(
                name: "Startup object",
                initialValue: "(Not set)",
                defaultValue: "(Not set)",
                enumValues: new[] { "(Not set)" },
                priority: 700,
                description: "Specifies the entry point for the executable.",
                page: "Application",
                category: "General"),

            new RadioSetting(
                name: "Resources",
                initialValue: "Icon and manifest",
                defaultValue: "Icon and manifest",
                priority: 800,
                description: "Specifies how application resources will be managed.",
                page: "Application",
                category: "Resources",
                options: new[]
                {
                    new RadioOption(
                        name: "Icon and manifest",
                        description: null,
                        settings: new Setting[]
                        {
                            // TODO make this IconBrowseSetting
                            new StringSetting(
                                name: "Icon path",
                                initialValue: "(Default Icon)",
                                defaultValue: "(Default Icon)",
                                description: "Path to the icon to embed into the output assembly.",
                                priority: 100,
                                page: "Application",
                                category: "Resources"),
                            // TODO make this FileBrowseSetting
                            // TODO this can appear disabled, find out why
                            new EnumSetting(
                                name: "Manifest path",
                                initialValue: "",
                                defaultValue: "",
                                description: "A manifest determines specific settings for an application. To embed a custom manifest, first add it to your project and then select it from the list.",
                                priority: 200,
                                page: "Application",
                                category: "Resources",
                                enumValues: new[] { "" })
                        }),
                    new RadioOption(
                        name: "Resource file",
                        description: null,
                        settings: new Setting[]
                        {
                            // TODO make this FileBrowseSetting
                            new StringSetting(
                                name: "Resource file path",
                                initialValue: "",
                                defaultValue: "",
                                description: "Specifies a Win32 res file to compile into this project.",
                                priority: 100,
                                page: "Application",
                                category: "Resources")
                        })
                }),

            //////
            ///// ASSEMBLY INFORMATION
            ////

            // TODO this section is disabled for .NET Core projects -- if we have time, determine whether there's anything in here we couldn't add later

//            new StringSetting(
//                name: "Assembly name",
//                initialValue: "ConsoleApp1",
//                priority: 100,
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
                initialValue: "TRACE",
                defaultValue: null,
                priority: 100,
                description: "A semicolon-delimited list of symbols to define for the compilation.",
                page: "Build",
                category: "General"),
            new BoolSetting(
                name: "Define DEBUG symbol",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to define the DEBUG compilation symbol.",
                priority: 200,
                page: "Build",
                category: "General"),
            new BoolSetting(
                name: "Define TRACE symbol",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to define the TRACE compilation symbol.",
                priority: 300,
                page: "Build",
                category: "General"),
            new EnumSetting(
                name: "Platform target",
                initialValue: "Any CPU",
                defaultValue: "Any CPU",
                enumValues: new[] { "Any CPU", "x86" },
                priority: 400,
                description: "The platform to target in this project configuration.",
                page: "Build",
                category: "General"),
            new EnumSetting(
                name: "Nullable reference types",
                initialValue: "Enable",
                defaultValue: "Disable",
                enumValues: new[] { "Disable", "Enable", "Warnings", "Annotations" },
                priority: 500,
                description: "Controls use of nullable annotations and warnings.",
                page: "Build",
                category: "General"),
            // TODO this is disabled in .NET Core -- why?
            new BoolSetting(
                name: "Prefer 32-bit",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether to prefer 32-bit when available.",
                priority: 600,
                page: "Build",
                category: "General"),
            new BoolSetting(
                name: "Unsafe code",
                initialValue: false,
                defaultValue: false,
                description: "Allow unsafe code in this project.",
                priority: 700,
                page: "Build",
                category: "General"),
            new BoolSetting(
                name: "Optimize code",
                initialValue: false,
                defaultValue: false,
                description: "Produce optimized output. Optimized binaries may be harder to debug.",
                priority: 800,
                page: "Build",
                category: "General"),

            //////
            ///// ERRORS AND WARNINGS
            ////
            
            new EnumSetting(
                name: "Warning level",
                initialValue: "4",
                defaultValue: "4",
                enumValues: new[] { "0", "1", "2", "3", "4" },
                priority: 100,
                description: "Sets the warning level, where higher levels produce more warnings.",
//              readMore: "https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/warn-compiler-option",
                page: "Build",
                category: "Errors and warnings"),
            new StringSetting(
                name: "Suppress specific warnings",
                initialValue: "1701;1702",
                defaultValue: "1701;1702",
                priority: 200,
                description: "A semicolon-delimited list of warning codes to suppress.",
                page: "Build",
                category: "Errors and warnings"),
            new EnumSetting(
                name: "Warnings as errors",
                initialValue: "Specific warnings",
                defaultValue: "Specific warnings",
                enumValues: new[] { "None", "All", "Specific warnings" },
                priority: 300,
                description: "Controls which warnings are treated as errors.",
                page: "Build",
                category: "Errors and warnings"),
            new StringSetting(
                name: "Treat specific warnings as errors",
                initialValue: "NU1605",
                defaultValue: "NU1605",
                priority: 400,
                description: "A semicolon-delimited list of warning codes to treat as errors.",
                page: "Build",
                category: "Errors and warnings"),

            //////
            ///// OUTPUT
            ////
            
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "Output path",
                initialValue: "",
                defaultValue: "",
                priority: 100,
                description: "Relative destination path for build output.",
                page: "Build",
                category: "Output"),
            // TODO make this FileBrowseSetting
            new StringSetting(
                name: "XML documentation path",
                initialValue: "",
                defaultValue: "",
                priority: 200,
                description: "Relative path to the output XML documentation. Clear to disable generation.",
                page: "Build",
                category: "Output"),
            // TODO this is disabled in .NET Core -- why?
            new BoolSetting(
                name: "Register for COM interop",
                initialValue: false,
                defaultValue: false,
                description: "Produce optimized output. Optimized binaries may be harder to debug.",
                priority: 300,
                page: "Build",
                category: "Output"),
            new EnumSetting(
                name: "Generate serialization assembly",
                initialValue: "Auto",
                defaultValue: "Auto",
                enumValues: new[] { "Auto", "On", "Off" },
                priority: 400,
                description: null,
                page: "Build",
                category: "Output"),

            //////
            ///// ADVANCED
            ////
            
            // TODO these settings are not configuration-dependent, so should probably go in their own page

            new LinkAction(
                name: "Language version",
                description: "Why can't I select a difference C# version?",
                priority: 100,
                page: "Build",
                category: "Advanced"),
            new EnumSetting(
                name: "Internal compiler error reporting",
                initialValue: "Prompt",
                defaultValue: "Prompt",
                enumValues: new[] { "None", "Prompt", "Send", "Queue" },
                priority: 200,
                description: null,
                page: "Build",
                category: "Advanced"),
            new BoolSetting(
                name: "Overflow checking",
                initialValue: false,
                defaultValue: false,
                description: "Enable arithmetic overflow checking at runtime.",
                priority: 300,
                page: "Build",
                category: "Advanced"),
            new EnumSetting(
                name: "Debugging information",
                initialValue: "Portable",
                defaultValue: "Portable",
                enumValues: new[] { "None", "Full", "Pdb-only", "Portable", "Embedded" },
                priority: 400,
                description: null,
                page: "Build",
                category: "Advanced"),
            new EnumSetting(
                name: "File alignment",
                initialValue: "512",
                defaultValue: "512",
                enumValues: new[] { "512", "1024", "2048", "4096", "8192" },
                priority: 500,
                description: null,
                page: "Build",
                category: "Advanced"),
            new StringSetting(
                name: "Library base address",
                initialValue: "0x11000000",
                defaultValue: "0x11000000",
                priority: 600,
                description: null,
                page: "Build",
                category: "Advanced"),

            /////////////
            //////////// BUILD EVENTS
            ///////////

            //////
            ///// GENERAL
            ////

            // TODO both these build events can be edited in a pop-out editor with macro support
            new MultiLineStringSetting(
                name: "Pre-build event",
                initialValue: "",
                defaultValue: "",
                priority: 100,
                description: "Commands to execute before a build occurs.",
                page: "Build Events",
                category: "General"),
            new MultiLineStringSetting(
                name: "Post-build event",
                initialValue: "",
                defaultValue: "",
                priority: 200,
                description: "Commands to execute after a build completes.",
                page: "Build Events",
                category: "General"),
            new EnumSetting(
                name: "Run the post-build event",
                initialValue: "On successful build",
                defaultValue: "On successful build",
                enumValues: new[] { "Always", "On successful build", "When the build updates the project output" },
                priority: 300,
                description: "Controls when any post-build event is executed.",
                page: "Build Events",
                category: "General"),

            /////////////
            //////////// PACKAGING
            ///////////

            //////
            ///// GENERAL
            ////

            new BoolSetting(
                name: "Generate NuGet package on build",
                initialValue: false,
                defaultValue: false,
                description: "Specifies whether a NuGet package should be produced in the output directory when the project is build.",
                priority: 100,
                page: "Packaging",
                category: "General"),
            new BoolSetting(
                name: "Require license acceptance",
                initialValue: false,
                defaultValue: false,
                description: "Controls whether consumers of the generated package are presented with a license acceptance prompt when adding a reference to this package.",
                priority: 200,
                page: "Packaging",
                category: "General"),
            new StringSetting(
                name: "Package ID",
                initialValue: "ConsoleApp1",
                defaultValue: "ConsoleApp1",
                priority: 300,
                description: null,
                page: "Packaging",
                category: "General"),
            // TODO VersionSetting (note -- has different validation rules to assembly/file versions)
            new StringSetting(
                name: "Package version",
                initialValue: "1.0.0",
                defaultValue: "1.0.0",
                priority: 400,
                description: null,
                page: "Packaging",
                category: "General"),
            new StringSetting(
                name: "Authors",
                initialValue: "ConsoleApp1",
                defaultValue: "ConsoleApp1",
                priority: 500,
                description: null,
                page: "Packaging",
                category: "General"),
            new StringSetting(
                name: "Company",
                initialValue: "ConsoleApp1",
                defaultValue: "ConsoleApp1",
                priority: 600,
                description: null,
                page: "Packaging",
                category: "General"),
            new StringSetting(
                name: "Product",
                initialValue: "ConsoleApp1",
                defaultValue: "ConsoleApp1",
                priority: 700,
                description: null,
                page: "Packaging",
                category: "General"),
            new MultiLineStringSetting(
                name: "Description",
                initialValue: "",
                defaultValue: null,
                priority: 800,
                description: null,
                page: "Packaging",
                category: "General"),
            new StringSetting(
                name: "Copyright",
                initialValue: "",
                defaultValue: null,
                priority: 900,
                description: null,
                page: "Packaging",
                category: "General"),
            new RadioSetting(
                name: "License specification",
                initialValue: "None",
                defaultValue: "None",
                priority: 1000,
                description: "Controls how the package's license is specified.",
                page: "Packaging",
                category: "General",
                options: new[]
                {
                    new RadioOption(
                        name: "None",
                        description: "No license is specified for this package.",
                        settings: Array.Empty<Setting>()),
                    new RadioOption(
                        name: "Expression",
                        description: "The license is specified using a SPDX expression",
                        settings: new Setting[]
                        {
                            // TODO provide some examples for auto-complete: Apache-2.0;MIT;...
                            new StringSetting(
                                name: "License expression",
                                initialValue: "",
                                defaultValue: null,
                                description: "The SPDX expression that specifies the package's license.",
                                priority: 100,
                                page: "Packaging",
                                category: "General"),
                            new LinkAction(
                                // https://spdx.org/licenses/
                                name: "Read about SPDX license expressions",
                                priority: 200,
                                page: "Packaging",
                                category: "General")
                        }),
                    new RadioOption(
                        name: "File",
                        description: "The license is provided in a file which will be included in the package.",
                        settings: new Setting[]
                        {
                            // TODO make this FileBrowseSetting
                            new StringSetting(
                                name: "License file path",
                                initialValue: "",
                                defaultValue: null,
                                description: "The path to the license file to include in the package. May be relative to the project directory.",
                                priority: 100,
                                page: "Packaging",
                                category: "General")
                        })
                }),
                // TODO make this IconBrowseSetting
                new StringSetting(
                    name: "Package icon file",
                    initialValue: "",
                    defaultValue: null,
                    description: "Path to the icon to include in and use for the package.",
                    priority: 1100,
                    page: "Packaging",
                    category: "General"),
                new StringSetting(
                    name: "Repository URL",
                    initialValue: "",
                    defaultValue: null,
                    priority: 1200,
                    description: null, // TODO describe what this URL means
                    page: "Packaging",
                    category: "General"),
                // TODO provide feedback about valid URLs here
                new StringSetting(
                    name: "Repository type",
                    initialValue: "",
                    defaultValue: null,
                    priority: 1300,
                    description: null,
                    page: "Packaging",
                    category: "General"),
                new StringSetting(
                    name: "Tags",
                    initialValue: "",
                    defaultValue: null,
                    priority: 1400,
                    description: null, // TODO describe how this is delimited
                    page: "Packaging",
                    category: "General"),
                new MultiLineStringSetting(
                    name: "Release notes",
                    initialValue: "",
                    defaultValue: null,
                    priority: 1500,
                    description: null,
                    page: "Packaging",
                    category: "General"),
                // TODO this is a combo box with many languages listed
                new StringSetting(
                    name: "Assembly neutral language",
                    initialValue: "(None)",
                    defaultValue: "(None)",
                    priority: 1600,
                    description: null,
                    page: "Packaging",
                    category: "General"),
                // TODO VersionSetting
                new StringSetting(
                    name: "Assembly version",
                    initialValue: "1.0.0.0",
                    defaultValue: "1.0.0.0",
                    priority: 1700,
                    description: null,
                    page: "Packaging",
                    category: "General"),
                // TODO VersionSetting
                new StringSetting(
                    name: "Assembly file version",
                    initialValue: "1.0.0.0",
                    defaultValue: "1.0.0.0",
                    priority: 1800,
                    description: null,
                    page: "Packaging",
                    category: "General"),
 
                /////////////
                //////////// PACKAGING
                ///////////

                //////
                ///// GENERAL
                ////
                
                new RadioSetting(
                    name: "Launch type",
                    description: null,
                    initialValue: "Project",
                    defaultValue: "Project",
                    priority: 100,
                    page: "Debug",
                    category: "General",
                    options: new[]
                    {
                        new RadioOption(
                            name: "Project",
                            description: null,
                            settings: Array.Empty<Setting>()),
                        new RadioOption(
                            name: "Executable",
                            description: null,
                            settings: new Setting[]
                            {
                                // TODO make this FileBrowseSetting
                                new StringSetting(
                                    name: "Executable path",
                                    initialValue: "",
                                    defaultValue: null,
                                    description: "Path to the executable to debug.",
                                    priority: 100,
                                    page: "Debug",
                                    category: "General"),
                            })
                    }),
                new StringSetting(
                    name: "Application arguments",
                    initialValue: "",
                    defaultValue: null,
                    description: "Arguments to be passed to the launched application.",
                    priority: 200,
                    page: "Debug",
                    category: "General"),
                // TODO make this FileBrowseSetting
                new StringSetting(
                    name: "Working directory",
                    initialValue: "",
                    defaultValue: null,
                    description: "Absolute path to the working directory.",
                    priority: 300,
                    page: "Debug",
                    category: "General"),
                new BoolSetting(
                    name: "Use remote machine",
                    initialValue: false,
                    defaultValue: false,
                    description: "The debug target is on a remote machine.",
                    priority: 400,
                    page: "Debug",
                    category: "General",
                    trueSettings: new Setting[]
                    {
                        // TODO make this RemoteMachineSetting, with support for the 'Find' button
                        new StringSetting(
                            name: "Remote machine host name",
                            initialValue: "",
                            defaultValue: null,
                            description: null,
                            priority: 100,
                            page: "Debug",
                            category: "General"),
                        new EnumSetting(
                            name: "Authentication mode",
                            initialValue: "None",
                            defaultValue: "None",
                            description: null,
                            priority: 200,
                            enumValues: new[] { "None", "Windows" },
                            page: "Debug",
                            category: "General")
                    }),
                // TODO NameValueListSetting
                new StringSetting(
                    name: "Environment variables",
                    initialValue: "",
                    defaultValue: "",
                    description: "Specifies environment variables to be set for the launched application.",
                    priority: 500,
                    page: "Debug",
                    category: "General"),
                new BoolSetting(
                    name: "Native code debugging",
                    initialValue: false,
                    defaultValue: false,
                    description: "Enable native code debugging.",
                    priority: 600,
                    page: "Debug",
                    category: "General"),
                new BoolSetting(
                    name: "SQL Server debugging",
                    initialValue: false,
                    defaultValue: false,
                    description: "Enable SQL Server debugging.",
                    priority: 700,
                    page: "Debug",
                    category: "General"),

                /////////////
                //////////// SIGNING
                ///////////

                //////
                ///// GENERAL
                ////
                
                new BoolSetting(
                    name: "Signing",
                    initialValue: false,
                    defaultValue: false,
                    description: "Sign the project's output assembly.",
                    priority: 100,
                    page: "Signing",
                    category: "General",
                    trueSettings: new Setting[]
                    {
                        // TODO StrongNameKeySetting -- with new/add and change password actions
                        new StringSetting(
                            name: "Key file path",
                            initialValue: "",
                            defaultValue: null,
                            description: "Choose a string name key file",
                            priority: 100,
                            page: "Signing",
                            category: "General"),
                        new BoolSetting(
                            name: "Delay signing",
                            initialValue: false,
                            defaultValue: false,
                            description: "Delay sign the assembly. When enabled the project will not run or be debuggable.",
                            priority: 200,
                            page: "Signing",
                            category: "General")
                    }),

                /////////////
                //////////// CODE ANALYSIS
                ///////////

                //////
                ///// ANALYZERS
                ////
                
                new LinkAction(
                    name: "What are the benefits of source code analyzers?",
                    priority: 100,
                    page: "Code Analysis",
                    category: "Analyzers"),
                new BoolSetting(
                    name: "Run on build",
                    initialValue: false,
                    defaultValue: false,
                    description: "Run analyzers during build.",
                    priority: 200,
                    page: "Code Analysis",
                    category: "Analyzers"),
                new BoolSetting(
                    name: "Run live analysis",
                    initialValue: false,
                    defaultValue: false,
                    description: "Run analyzers live in the IDE.",
                    priority: 300,
                    page: "Code Analysis",
                    category: "Analyzers"),
       };
    }
}