using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingContext
    {
        private readonly ImmutableArray<SettingCondition> _settingConditions;
        private readonly bool _requireConditionMatches;

        public IImmutableDictionary<string, ImmutableArray<string>> Dimensions { get; }

        /// <summary>
        /// Gets whether any project configuration dimension contains more than one potential value.
        /// </summary>
        public bool HasConfigurableDimensions { get; }

        public IReadOnlyList<Setting> Settings { get; }

        public SettingContext(IImmutableDictionary<string, ImmutableArray<string>> dimensions, ImmutableArray<SettingCondition> settingConditions, bool requireConditionMatches, IReadOnlyList<Setting> settings)
        {
            _settingConditions = settingConditions;
            _requireConditionMatches = requireConditionMatches;
            Dimensions = dimensions;
            Settings = settings;

            HasConfigurableDimensions = dimensions.Any(entry => entry.Value.Length > 1);

            var settingByIdentity = settings.ToDictionary(setting => setting.Identity);

            foreach (var condition in settingConditions)
            {
                if (!settingByIdentity.TryGetValue(condition.Source, out Setting source) && _requireConditionMatches)
                    throw new Exception("Unknown source: " + condition.Source);
                if (!settingByIdentity.TryGetValue(condition.Target, out Setting target) && _requireConditionMatches)
                    throw new Exception("Unknown target: " + condition.Target);

                if (source != null && target != null)
                {
                    source.AddDependentTarget(target, condition.SourceValue);
                }
            }

            foreach (var setting in Settings)
            {
                setting.Initialize(this);
            }
        }

        public SettingContext Clone()
        {
            return new SettingContext(
                Dimensions,
                _settingConditions,
                _requireConditionMatches, Settings.Select(setting => setting.Clone()).ToImmutableArray());
        }
    }
}