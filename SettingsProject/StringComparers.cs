using System;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal static class StringComparers
    {
        public static StringComparer ConfigurationDimensionNames { get; } = StringComparer.OrdinalIgnoreCase;
        public static StringComparer ConfigurationDimensionValues { get; } = StringComparer.OrdinalIgnoreCase;
        public static StringComparer PropertyValues { get; } = StringComparer.OrdinalIgnoreCase;
    }

    internal static class StringComparisons
    {
        public static StringComparison SearchText { get; } = StringComparison.CurrentCultureIgnoreCase;
    }
}