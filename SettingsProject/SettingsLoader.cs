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
                description: "Specifies whether this project builds for multiple target frameworks.",
                priority: 300,
                page: "Application",
                category: "General"),
//                trueSettings: new Setting[]
//                {
////            new FlagsEnumSetting(
////                name: "Target frameworks",
////                initialValue: "netcoreapp3.0",
////                defaultValue: null,
////                enumValues: new[] { "Console Application", "Windows Application", "Class Library" },
////                priority: 4,
////                description: "Specifies whether the output is executable, and whether is runs in a console or as a desktop application.",
////                page: "Application",
////                category: "General",
////                // TODO what about netstandard, others, do we really want to have a fixed set here?
////                enumValues: new[] { ".net5", ".netcoreapp3.1", ".netcoreapp3.0", ".netcoreapp2.2", ".netcoreapp2.1", ".netcoreapp2.0", ".netcoreapp1.1", ".netcoreapp1.0" }),
//                },
//                falseSettings: new Setting[]
//                {
//                    new EnumSetting(
//                        name: "Target framework",
//                        initialValue: ".NET 5",
//                        defaultValue: null,
//                        priority: 3,
//                        description: "Specifies the semicolon-delimited list of frameworks that this project will target. Often just a single value.",
//                        page: "Application",
//                        category: "General",
//                        enumValues: new[] { ".NET 5", ".NET Core 3.1", ".NET Core 3.0", ".NET Core 2.2", ".NET Core 2.1", ".NET Core 2.0", ".NET Core 1.1", ".NET Core 1.0" }),
//                }),
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

//            new RadioSetting(
//                name: "Resources",
//                initialValue: "Icon and manifest",
//                defaultValue: "Icon and manifest",
//                priority: 800,
//                description: "Specifies how application resources will be managed.",
//                page: "Application",
//                category: "Resources",
//                options: new[]
//                {
//                    new RadioOption(
//                        name: "Icon and manifest",
//                        description: "A manifest determines specific settings for an application.",
//                        settings: new Setting[]
//                        {
//                            // TODO make this IconBrowseSetting
//                            new StringSetting(
//                                name: "Icon path",
//                                initialValue: "(Default Icon)",
//                                defaultValue: "(Default Icon)",
//                                description: "Path to the icon to embed into the output assembly.",
//                                priority: 100,
//                                page: "Application",
//                                category: "Resources"),
//                            // TODO make this FileBrowseSetting
//                            // TODO this can appear disabled, find out why
//                            new EnumSetting(
//                                name: "Manifest path",
//                                initialValue: "",
//                                defaultValue: "",
//                                description: "To embed a custom manifest, first add it to your project and then select it from the list.",
//                                priority: 200,
//                                page: "Application",
//                                category: "Resources",
//                                enumValues: new[] { "" })
//                        }),
//                    new RadioOption(
//                        name: "Resource file",
//                        description: "Specifies a Win32 res file to compile into this project.",
//                        settings: new Setting[]
//                        {
//                            // TODO make this FileBrowseSetting
//                            new StringSetting(
//                                name: "Resource file path",
//                                initialValue: "",
//                                defaultValue: "",
//                                description: "",
//                                priority: 100,
//                                page: "Application",
//                                category: "Resources")
//                        })
//                }),

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
        };
    }
}