using System.Collections.Generic;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class EditorSpecification
    {
        public string TypeName { get; }

        public IReadOnlyDictionary<string, string> Metadata { get; }

        public EditorSpecification(string typeName, IReadOnlyDictionary<string, string> metadata)
        {
            TypeName = typeName;
            Metadata = metadata;
        }
    }
}
