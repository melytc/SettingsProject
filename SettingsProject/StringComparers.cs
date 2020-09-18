using System;

#nullable enable

namespace SettingsProject
{
    internal static class StringComparers
    {
        public static StringComparer ConfigurationDimensionNames { get; } = StringComparer.OrdinalIgnoreCase;
        public static StringComparer ConfigurationDimensionValues { get; } = StringComparer.OrdinalIgnoreCase;
        public static StringComparer SettingValues { get; } = StringComparer.OrdinalIgnoreCase;
    }

    internal static class StringComparisons
    {
        public static StringComparison SearchText { get; } = StringComparison.CurrentCultureIgnoreCase;
    }
}