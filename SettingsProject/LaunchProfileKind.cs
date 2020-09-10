using System.Collections.Immutable;
using System.Windows.Media;

#nullable enable

namespace SettingsProject
{
    internal sealed class LaunchProfileKind
    {
        public string Name { get; }

        public ImmutableArray<SettingMetadata> Metadata { get; }

        public DrawingBrush IconBrush { get; }

        public LaunchProfileKind(string name, ImmutableArray<SettingMetadata> metadata, DrawingBrush iconBrush)
        {
            Name = name;
            Metadata = metadata;
            IconBrush = iconBrush;
        }
    }
}