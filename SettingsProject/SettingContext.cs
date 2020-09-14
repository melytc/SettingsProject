using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

#nullable enable

namespace SettingsProject
{
    internal sealed class SettingContextBuilder : IEnumerable
    {
        private readonly IImmutableDictionary<string, ImmutableArray<string>> _dimensions;
        private readonly ImmutableArray<SettingCondition> _conditions;
        private readonly bool _requireConditionMatches;
        private readonly List<Setting> _settings = new List<Setting>();
        private int _built;

        public SettingContextBuilder(IImmutableDictionary<string, ImmutableArray<string>> dimensions, ImmutableArray<SettingCondition> conditions, bool requireConditionMatches)
        {
            _dimensions = dimensions;
            _conditions = conditions;
            _requireConditionMatches = requireConditionMatches;
        }

        public void Add(Setting setting)
        {
            if (_built != 0)
                throw new InvalidOperationException("Already built.");

            // TODO validate values match dimensions?
            _settings.Add(setting);
        }

        public SettingContext Build()
        {
            if (Interlocked.Increment(ref _built) != 1)
                throw new InvalidOperationException("Already built.");

            var settingByIdentity = _settings.ToDictionary(setting => setting.Identity);

            foreach (var condition in _conditions)
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

            return new SettingContext(_dimensions, _settings, _conditions, _requireConditionMatches);
        }

        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
    }

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

        public SettingContext(IImmutableDictionary<string, ImmutableArray<string>> dimensions, IReadOnlyList<Setting> settings, ImmutableArray<SettingCondition> settingConditions, bool requireConditionMatches)
        {
            _settingConditions = settingConditions;
            _requireConditionMatches = requireConditionMatches;
            Dimensions = dimensions;
            Settings = settings;

            HasConfigurableDimensions = dimensions.Any(entry => entry.Value.Length > 1);
        }

        public SettingContext Clone()
        {
            var context = new SettingContextBuilder(Dimensions, _settingConditions, _requireConditionMatches);

            foreach (var setting in Settings)
            {
                context.Add(setting.Clone());
            }

            return context.Build();
        }
    }
}