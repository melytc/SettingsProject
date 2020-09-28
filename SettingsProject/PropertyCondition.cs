#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal readonly struct PropertyCondition
    {
        public PropertyIdentity Source { get; }
        public object SourceValue { get; }
        public PropertyIdentity Target { get; }

        public PropertyCondition(PropertyIdentity source, object sourceValue, PropertyIdentity target)
        {
            Source = source;
            SourceValue = sourceValue;
            Target = target;
        }

        public override string ToString()
        {
            return $"{nameof(Source)}: {Source}, {nameof(SourceValue)}: {SourceValue}, {nameof(Target)}: {Target}";
        }
    }
}