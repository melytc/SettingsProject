using System.ComponentModel;

#nullable enable

namespace SettingsProject
{
    internal readonly struct SupportedValue
    {
        public string DisplayName { get; }
        public string Value { get; }

        public SupportedValue([Localizable(true)] string value)
        {
            Value = DisplayName = value;
        }

        public SupportedValue([Localizable(true)] string displayName, string value)
        {
            DisplayName = displayName;
            Value = value;
        }

        public override string ToString() => Value == DisplayName ? Value : $"{DisplayName} ({Value})";
    }
}