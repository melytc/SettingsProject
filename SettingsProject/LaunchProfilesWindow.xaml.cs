using System;
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
    internal partial class LaunchProfilesWindow
    {
        private static readonly IReadOnlyList<SettingCondition> Conditions = new[]
        {
            new SettingCondition(
                source: new SettingIdentity("Debug", "General", "Use remote machine"),
                sourceValue: true,
                target: new SettingIdentity("Debug", "General", "Remote machine host name")),
            new SettingCondition(
                source: new SettingIdentity("Debug", "General", "Use remote machine"),
                sourceValue: true,
                target: new SettingIdentity("Debug", "General", "Authentication mode")),
        };

        private static readonly ImmutableArray<SettingMetadata> ExecutableKindSettingMetadata = ImmutableArray.Create(
                // TODO make this FileBrowseSetting
                new SettingMetadata(
                    name: "Executable path",
                    description: "Path to the executable to debug.",
                    page: "Debug",
                    category: "General",
                    priority: 100,
                    editorType: "String",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                new SettingMetadata(
                    name: "Application arguments",
                    description: "Arguments to be passed to the launched application.",
                    page: "Debug",
                    category: "General",
                    priority: 200,
                    editorType: "String",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                // TODO make this FileBrowseSetting
                new SettingMetadata(
                    name: "Working directory",
                    description: "Absolute path to the working directory.",
                    page: "Debug",
                    category: "General",
                    priority: 300,
                    editorType: "String",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                new SettingMetadata(
                    name: "Use remote machine",
                    description: "The debug target is on a remote machine.",
                    page: "Debug",
                    category: "General",
                    priority: 400,
                    editorType: "Bool",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                // TODO make this RemoteMachineSetting, with support for the 'Find' button
                new SettingMetadata(
                    name: "Remote machine host name",
                    description: null,
                    page: "Debug",
                    category: "General",
                    priority: 410,
                    editorType: "String",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                new SettingMetadata(
                    name: "Authentication mode",
                    description: null,
                    page: "Debug",
                    category: "General",
                    priority: 420,
                    editorType: "Enum",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray.Create("None", "Windows")),
                // TODO NameValueListSetting
                new SettingMetadata(
                    name: "Environment variables",
                    description: "Specifies environment variables to be set for the launched application.",
                    page: "Debug",
                    category: "General",
                    priority: 500,
                    editorType: "String",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                new SettingMetadata(
                    name: "Native code debugging",
                    description: "Enable native code debugging.",
                    page: "Debug",
                    category: "General",
                    priority: 600,
                    editorType: "Bool",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                new SettingMetadata(
                    name: "SQL Server debugging",
                    description: "Enable SQL Server debugging.",
                    page: "Debug",
                    category: "General",
                    priority: 700,
                    editorType: "Bool",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty));

        private static readonly ImmutableArray<SettingMetadata> ProjectKindSettingMetadata = ExecutableKindSettingMetadata.Skip(1).ToImmutableArray();

        private static readonly ImmutableArray<SettingMetadata> SnapshotDebuggerKindSettingMetadata = ImmutableArray.Create(
                // TODO editorType AzureResource
                new SettingMetadata(
                    name: "Azure resource",
                    description: "The Azure resource to use in your snapshot debugging session.",
                    page: "Debug",
                    category: "General",
                    priority: 100,
                    editorType: "Enum",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty),
                // TODO editorType AzureStorage
                new SettingMetadata(
                    name: "Azure Storage account",
                    description: "The Azure resource to use in your snapshot debugging session.",
                    page: "Debug",
                    category: "General",
                    priority: 200,
                    editorType: "Enum",
                    supportsPerConfigurationValues: false,
                    enumValues: ImmutableArray<string>.Empty));

        public LaunchProfilesWindow()
        {
            var projectKind = new LaunchProfileKind("Project", ProjectKindSettingMetadata, FindBrush("IconApplicationBrush"));
            var executableKind = new LaunchProfileKind("Executable", ExecutableKindSettingMetadata, FindBrush("IconExecuteBrush"));
            var snapshotDebuggerKind = new LaunchProfileKind("Snapshot Debugger", SnapshotDebuggerKindSettingMetadata, FindBrush("SnapshotDebuggerBrush"));

            var profileKinds = ImmutableArray.Create(projectKind, executableKind, snapshotDebuggerKind);

            var profiles = new ObservableCollection<LaunchProfileViewModel>
            {
                CreateLaunchProfileViewModel("My project", projectKind, new Dictionary<string, object>
                {
                    { "Executable path", "devenv.exe" }
                }),
                CreateLaunchProfileViewModel("devenv.exe", executableKind, new Dictionary<string, object>
                {
                    { "Executable path", "devenv.exe" },
                    { "Application arguments", "/rootSuffix Exp" }
                }),
                CreateLaunchProfileViewModel("My Snapshot", snapshotDebuggerKind, new Dictionary<string, object>
                {
                    // TODO
                })
            };

            DataContext = new LaunchProfilesWindowViewModel(profiles, profileKinds);

            InitializeComponent();

            static LaunchProfileViewModel CreateLaunchProfileViewModel(string name, LaunchProfileKind kind, Dictionary<string, object> initialValues)
            {
                var settings = kind.Metadata.Select(CreateSetting).ToImmutableArray();

                // TODO revisit how we model conditionality between settings

                var settingByIdentity = settings.ToDictionary(setting => setting.Identity);

                foreach (var condition in Conditions)
                {
                    if (!settingByIdentity.TryGetValue(condition.Source, out Setting? source))
                        continue; // throw new Exception("Unknown source: " + condition.Source);
                    if (!settingByIdentity.TryGetValue(condition.Target, out Setting? target))
                        continue; // throw new Exception("Unknown target: " + condition.Target);

                    source.AddDependentTarget(target, condition.SourceValue);
                }

                return new LaunchProfileViewModel(name, settings, kind);

                Setting CreateSetting(SettingMetadata metadata)
                {
                    if (!initialValues.TryGetValue(metadata.Name, out object value))
                    {
                        Assumes.NotNull(metadata.Editor);
                        value = metadata.Editor.GetDefaultValue(metadata);
                    }

                    return new Setting(metadata, ImmutableArray.Create<ISettingValue>(new UnconfiguredSettingValue(value)));
                }
            }

            static DrawingBrush FindBrush(string resourceKey)
            {
                var resource = Application.Current.FindResource(resourceKey);
                Assumes.NotNull(resource);
                return (DrawingBrush)resource;
            }
        }
    }
}
