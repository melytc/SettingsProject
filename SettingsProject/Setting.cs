using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettingsProject
{
    class Setting
    {
        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string Value { get; set; }
    }

    class SettingsViewModel
    {
        public List<Setting> Settings { get; } = new List<Setting>
        {
            new Setting("Assembly name", "ConsoleApp1"),
            new Setting("Default namespace", "ConsoleApp1"),
            new Setting("Target framework", ".NET Code 3.0"),
            new Setting("Output type", "Console Application"),
            new Setting("Startup object", "(Not set)")
        };
    }
}
