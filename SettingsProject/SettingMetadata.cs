#nullable enable

namespace SettingsProject
{
    /// <summary>
    /// Immutable data about a setting.
    /// </summary>
    internal sealed class SettingMetadata
    {
        public string Name { get; }

        public string Page { get; }

        public string Category { get; }

        public string? Description { get; }

        /// <summary>
        /// Relative priority of the setting, to use when ordering items in the UI.
        /// </summary>
        public int Priority { get; }

        public bool SupportsPerConfigurationValues { get; }

        // TODO Editors

        public SettingMetadata(string name, string page, string category, string? description, int priority, bool supportsPerConfigurationValues)
        {
            Name = name;
            Page = page;
            Category = category;
            Description = description;
            Priority = priority;
            SupportsPerConfigurationValues = supportsPerConfigurationValues;
        }

        public SettingIdentity Identity => new SettingIdentity(Page, Category, Name);
    }
}
