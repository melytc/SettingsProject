using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsProject
{
#nullable enable
    abstract class Setting
    {
        public string Name { get; }
        //public string? Description { get; }

        protected Setting(string name)
        {
            Name = name;
        }
    }

    class StringSetting : Setting
    {
        public StringSetting(string name, string value) : base(name)
        {
            Value = value;
        }

        public string Value { get; set; }
    }

    class SettingsViewModel
    {
        public List<Setting> Settings { get; } = new List<Setting>
        {
            new StringSetting("Assembly name", "ConsoleApp1"),
            new StringSetting("Default namespace", "ConsoleApp1"),
            new StringSetting("Target framework", ".NET Code 3.0"),
            new EnumSetting("Output type", new List<string>{ "Console Application", "Windows Application", "Class Library" }),
            new BoolSetting("Binding redirects", true, "Auto-generate binding redirects")
        };
    }

    class BoolSetting : Setting
    {
        public BoolSetting(string name, bool value, string description) : base(name)
        {
            Value = value;
            Description = description;
        }

        public bool Value { get; set; }
        public string Description { get; }
    }

    class EnumSetting : Setting
    {
        public List<string> EnumValues { get; }
        public string SelectedValue { get; set; }

        // Note: We might want to use IEnumValue here.
        public EnumSetting(string name, List<string> enumValues) : base(name)
        {
            EnumValues = enumValues;
            SelectedValue = enumValues[0];
        }
    }
}
