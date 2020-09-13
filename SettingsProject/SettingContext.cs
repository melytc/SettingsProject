using System.Collections.Generic;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingContext
    {
        private readonly Dictionary<SettingIdentity, Setting> _settingByIdentity = new Dictionary<SettingIdentity, Setting>();

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