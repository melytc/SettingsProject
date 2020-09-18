#nullable enable

namespace SettingsProject
{
    internal readonly struct SupportedValue
    {
        public string DisplayName { get; }
        public string Value { get; }

        public SupportedValue(string value)
        {
            Value = DisplayName = value;
        }

        public SupportedValue(string displayName, string value)
        {
            DisplayName = displayName;
            Value = value;
        }

        public override string ToString() => Value == DisplayName ? Value : $"{DisplayName} ({Value})";
    }
}