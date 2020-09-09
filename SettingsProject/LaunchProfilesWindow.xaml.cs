using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

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

        private static ImmutableArray<Setting> CreateSettings(LaunchProfileKind kind)
        {
            var settings = new Setting?[]
            {
                // TODO make this FileBrowseSetting
                kind == LaunchProfileKind.Executable
                    ? new Setting(
                        name: "Executable path",
                        description: "Path to the executable to debug.",
                        page: "Debug",
                        category: "General",
                        priority: 100,
                        editorType: "String",
                        new UnconfiguredSettingValue("devenv.exe"))
                    : null,
                new Setting(
                    name: "Application arguments",
                    description: "Arguments to be passed to the launched application.",
                    page: "Debug",
                    category: "General",
                    priority: 200,
                    editorType: "String",
                    new UnconfiguredSettingValue("/rootSuffix Exp")),
                // TODO make this FileBrowseSetting
                new Setting(
                    name: "Working directory",
                    description: "Absolute path to the working directory.",
                    page: "Debug",
                    category: "General",
                    priority: 300,
                    editorType: "String",
                    new UnconfiguredSettingValue("")),
                new Setting(
                    name: "Use remote machine",
                    description: "The debug target is on a remote machine.",
                    page: "Debug",
                    category: "General",
                    priority: 400,
                    editorType: "Bool",
                    new UnconfiguredSettingValue(false)),
                // TODO make this RemoteMachineSetting, with support for the 'Find' button
                new Setting(
                    name: "Remote machine host name",
                    description: null,
                    page: "Debug",
                    category: "General",
                    priority: 410,
                    editorType: "String",
                    new UnconfiguredSettingValue("")),
                new Setting(
                    name: "Authentication mode",
                    description: null,
                    page: "Debug",
                    category: "General",
                    priority: 420,
                    enumValues: ImmutableArray.Create("None", "Windows"),
                    editorType: "Enum",
                    value: new UnconfiguredSettingValue("None")),
                // TODO NameValueListSetting
                new Setting(
                    name: "Environment variables",
                    description: "Specifies environment variables to be set for the launched application.",
                    page: "Debug",
                    category: "General",
                    priority: 500,
                    editorType: "String",
                    new UnconfiguredSettingValue("")),
                new Setting(
                    name: "Native code debugging",
                    description: "Enable native code debugging.",
                    page: "Debug",
                    category: "General",
                    priority: 600,
                    editorType: "Bool",
                    new UnconfiguredSettingValue(false)),
                new Setting(
                    name: "SQL Server debugging",
                    description: "Enable SQL Server debugging.",
                    page: "Debug",
                    category: "General",
                    priority: 700,
                    editorType: "Bool",
                    new UnconfiguredSettingValue(false)),
            }.Where(setting => setting != null)!.ToImmutableArray<Setting>();

            var settingByIdentity = settings.ToDictionary(setting => setting.Identity);

            foreach (var condition in Conditions)
            {
                if (!settingByIdentity.TryGetValue(condition.Source, out Setting? source))
                    throw new Exception("Unknown source: " + condition.Source);
                if (!settingByIdentity.TryGetValue(condition.Target, out Setting? target))
                    throw new Exception("Unknown target: " + condition.Target);

                source.AddDependentTarget(target, condition.SourceValue);
            }

            return settings;
        }

        public LaunchProfilesWindow()
        {
            var profiles = new ObservableCollection<LaunchProfileViewModel>
            {
                new LaunchProfileViewModel("My project", CreateSettings(LaunchProfileKind.Project), LaunchProfileKind.Project),
                new LaunchProfileViewModel("devenv.exe", CreateSettings(LaunchProfileKind.Executable), LaunchProfileKind.Executable)
            };

            DataContext = new LaunchProfilesWindowViewModel(profiles);
            InitializeComponent();
        }
    }
}
