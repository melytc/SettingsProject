using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingContext
    {
        private readonly Dictionary<SettingIdentity, Setting> _settingByIdentity = new Dictionary<SettingIdentity, Setting>();

        public IImmutableDictionary<string, ImmutableArray<string>> Dimensions { get; }

        /// <summary>
        /// Gets whether any project configuration dimension contains more than one 
        /// </summary>
        public bool HasConfigurableDimensions { get; }

        public SettingContext(IImmutableDictionary<string, ImmutableArray<string>> dimensions)
        {
            Dimensions = dimensions;
            HasConfigurableDimensions = dimensions.Any(entry => entry.Value.Length > 1);
        }

        public void AddSetting(Setting setting)
        {
            _settingByIdentity.Add(setting.Identity, setting);
        }

        public Setting GetSetting(in SettingIdentity targetIdentity)
        {
            return _settingByIdentity[targetIdentity];
        }
    }
}