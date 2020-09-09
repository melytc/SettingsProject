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
                    ? new StringSetting(
                        name: "Executable path",
                        description: "Path to the executable to debug.",
                        page: "Debug",
                        category: "General",
                        priority: 100,
                        new UnconfiguredStringSettingValue("devenv.exe"))
                    : null,
                new StringSetting(
                    name: "Application arguments",
                    description: "Arguments to be passed to the launched application.",
                    page: "Debug",
                    category: "General",
                    priority: 200,
                    new UnconfiguredStringSettingValue("/rootSuffix Exp")),
                // TODO make this FileBrowseSetting
                new StringSetting(
                    name: "Working directory",
                    description: "Absolute path to the working directory.",
                    page: "Debug",
                    category: "General",
                    priority: 300,
                    new UnconfiguredStringSettingValue("")),
                new BoolSetting(
                    name: "Use remote machine",
                    description: "The debug target is on a remote machine.",
                    page: "Debug",
                    category: "General",
                    priority: 400,
                    new UnconfiguredBoolSettingValue(false)),
                // TODO make this RemoteMachineSetting, with support for the 'Find' button
                new StringSetting(
                    name: "Remote machine host name",
                    description: null,
                    page: "Debug",
                    category: "General",
                    priority: 410,
                    new UnconfiguredStringSettingValue("")),
                new EnumSetting(
                    name: "Authentication mode",
                    description: null,
                    page: "Debug",
                    category: "General",
                    priority: 420,
                    enumValues: ImmutableArray.Create("None", "Windows"),
                    new UnconfiguredEnumSettingValue("None")),
                // TODO NameValueListSetting
                new StringSetting(
                    name: "Environment variables",
                    description: "Specifies environment variables to be set for the launched application.",
                    page: "Debug",
                    category: "General",
                    priority: 500,
                    new UnconfiguredStringSettingValue("")),
                new BoolSetting(
                    name: "Native code debugging",
                    description: "Enable native code debugging.",
                    page: "Debug",
                    category: "General",
                    priority: 600,
                    new UnconfiguredBoolSettingValue(false)),
                new BoolSetting(
                    name: "SQL Server debugging",
                    description: "Enable SQL Server debugging.",
                    page: "Debug",
                    category: "General",
                    priority: 700,
                    new UnconfiguredBoolSettingValue(false)),
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
