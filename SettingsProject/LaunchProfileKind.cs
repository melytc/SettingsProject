using System.Collections.Immutable;
using System.ComponentModel;
using System.Windows.Media;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class LaunchProfileKind
    {
        public string DisplayName { get; }

        public ImmutableArray<PropertyMetadata> Metadata { get; }
        
        public ImmutableArray<PropertyCondition> Conditions { get; }

        // TODO this will likely become an ImageMoniker in VS
        public Drawing IconDrawing { get; }

        public LaunchProfileKind(
            [Localizable(true)] string displayName,
            ImmutableArray<PropertyMetadata> metadata,
            ImmutableArray<PropertyCondition> conditions,
            Drawing iconDrawing)
        {
            DisplayName = displayName;
            Metadata = metadata;
            Conditions = conditions;
            IconDrawing = iconDrawing;
        }
    }
}