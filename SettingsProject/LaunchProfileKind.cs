using System.Collections.Immutable;
using System.Windows.Media;

#nullable enable

namespace SettingsProject
{
    internal sealed class LaunchProfileKind
    {
        public string Name { get; }

        public ImmutableArray<SettingMetadata> Metadata { get; }
        
        public ImmutableArray<SettingCondition> Conditions { get; }

        // TODO this will likely become an ImageMoniker in VS
        public Drawing IconDrawing { get; }

        public LaunchProfileKind(
            string name,
            ImmutableArray<SettingMetadata> metadata,
            ImmutableArray<SettingCondition> conditions,
            Drawing iconDrawing)
        {
            Name = name;
            Metadata = metadata;
            Conditions = conditions;
            IconDrawing = iconDrawing;
        }
    }
}