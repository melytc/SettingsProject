using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Media;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class LaunchProfileKind
    {
        public string DisplayName { get; }

        public ImmutableArray<SettingMetadata> Metadata { get; }
        
        public ImmutableArray<SettingCondition> Conditions { get; }

        // TODO this will likely become an ImageMoniker in VS
        public Drawing IconDrawing { get; }

        public LaunchProfileKind(
            [Localizable(true)] string displayName,
            ImmutableArray<SettingMetadata> metadata,
            ImmutableArray<SettingCondition> conditions,
            Drawing iconDrawing)
        {
            DisplayName = displayName;
            Metadata = metadata;
            Conditions = conditions;
            IconDrawing = iconDrawing;
        }
    }
}