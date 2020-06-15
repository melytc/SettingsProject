using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsProject
{
    abstract class Setting
    {
        public string Name { get; }

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
            new StringSetting("Output type", "Console Application"),
            new BoolSetting("Auto-generate binding redirects", true)
        };
    }

    class BoolSetting : Setting
    {
        public BoolSetting(string name, bool value) : base(name)
        {
            Value = value;
        }

        public bool Value { get; set; }
    }
}
