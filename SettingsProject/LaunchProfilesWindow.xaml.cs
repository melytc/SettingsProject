﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft;

#nullable enable

namespace SettingsProject
{
    internal partial class LaunchProfilesWindow
    {
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

        public LaunchProfilesWindow()
        {
            var conditions = ImmutableArray.Create(
                new SettingCondition(
                    source: UseRemoteMachine.Identity,
                    sourceValue: true,
                    target: RemoteMachineHostName.Identity),
                new SettingCondition(
                    source: UseRemoteMachine.Identity,
                    sourceValue: true,
                    target: AuthenticationMode.Identity),
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

            var projectKind = new LaunchProfileKind("Project", projectKindSettingMetadata, FindDrawing("IconApplicationDrawing"));
            var executableKind = new LaunchProfileKind("Executable", executableKindSettingMetadata, FindDrawing("IconExecuteDrawing"));
            var snapshotDebuggerKind = new LaunchProfileKind("Snapshot Debugger", snapshotDebuggerKindSettingMetadata, FindDrawing("SnapshotDebuggerDrawing"));
            var iisKind = new LaunchProfileKind("IIS", iisKindSettingMetadata, FindDrawing("IISDrawing"));
            var iisExpressKind = new LaunchProfileKind("IIS Express", iisExpressKindSettingMetadata, FindDrawing("IISExpressDrawing"));

            var enumValuesBySetting = new Dictionary<SettingIdentity, ImmutableArray<string>>
            {
                { AuthenticationMode.Identity, ImmutableArray.Create("None", "Windows") },
                { IisExpressBitness.Identity, ImmutableArray.Create("Default", "x64", "x86") },
                { HostingModel.Identity, ImmutableArray.Create("Default (In Process)", "In Process", "Out of Process") }
            };

            var defaultValueByEditorType = new Dictionary<string, object>
            {
                {"String", ""},
                {"MultiLineString", ""},
                {"Bool", false},
                {"Enum", ""},
                {"FileBrowse", ""},
                {"LinkAction", ""}
            };

            var profileKinds = ImmutableArray.Create(projectKind, executableKind, snapshotDebuggerKind, iisKind, iisExpressKind);

            var profiles = new ObservableCollection<LaunchProfileViewModel>
            {
                CreateLaunchProfileViewModel("My project", projectKind, new Dictionary<SettingIdentity, object>
                {
                    { ApplicationArguments.Identity, "/foo /bar" }
                }),
                CreateLaunchProfileViewModel("devenv.exe", executableKind, new Dictionary<SettingIdentity, object>
                {
                    { ExecutablePath.Identity, "devenv.exe" },
                    { ApplicationArguments.Identity, "/rootSuffix Exp" }
                }),
                CreateLaunchProfileViewModel("My Snapshot", snapshotDebuggerKind, new Dictionary<SettingIdentity, object>
                {
                    // TODO
                }),
                CreateLaunchProfileViewModel("My IIS", iisKind, new Dictionary<SettingIdentity, object>
                {
                    { AppUrl.Identity, "http://localhost:52531" },
                    { LaunchBrowser.Identity, true }
                }),
                CreateLaunchProfileViewModel("My IIS Express", iisExpressKind, new Dictionary<SettingIdentity, object>
                {
                    { AppUrl.Identity, "http://localhost:52531" },
                    { LaunchBrowser.Identity, true }
                })
            };

            DataContext = new LaunchProfilesWindowViewModel(profiles, profileKinds);

            InitializeComponent();

            LaunchProfileViewModel CreateLaunchProfileViewModel(string name, LaunchProfileKind kind, Dictionary<SettingIdentity, object> initialValues)
            {
                var contextBuilder = new SettingContextBuilder(SettingsLoader.DefaultConfigurationDictionary, conditions, requireConditionMatches: false);

                foreach (var metadata in kind.Metadata)
                {
                    contextBuilder.Add(CreateSetting(metadata));
                }

                return new LaunchProfileViewModel(name, kind, contextBuilder.Build());

                Setting CreateSetting(SettingMetadata metadata)
                {
                    // Debug launch profile values are unconfigured, so use an empty dimensions array
                    var configurationDimensions = ImmutableArray<string>.Empty;

                    var settingValue = new SettingValue(configurationDimensions, value: "");

                    if (enumValuesBySetting.TryGetValue(metadata.Identity, out ImmutableArray<string> enumValues))
                    {
                        settingValue.EnumValues = enumValues;
                        settingValue.Value = enumValues.First();
                    }

                    if (initialValues.TryGetValue(metadata.Identity, out object value) ||
                        defaultValueByEditorType.TryGetValue(metadata.EditorType, out value))
                    {
                        settingValue.Value = value;
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
