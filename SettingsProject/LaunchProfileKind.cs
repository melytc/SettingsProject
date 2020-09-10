using System.Collections.Immutable;
using System.Windows.Media;

#nullable enable

namespace SettingsProject
{
    internal sealed class LaunchProfileKind
    {
        public string Name { get; }

        public ImmutableArray<SettingMetadata> Metadata { get; }

        public Drawing IconDrawing { get; }

        public LaunchProfileKind(string name, ImmutableArray<SettingMetadata> metadata, Drawing iconDrawing)
        {
            Name = name;
            Metadata = metadata;
            IconDrawing = iconDrawing;
        }
    }
}