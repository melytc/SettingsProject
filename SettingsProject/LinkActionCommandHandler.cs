using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

#nullable enable

namespace SettingsProject
{
    internal static class LinkActionCommandHandler
    {
        private static readonly Dictionary<string, Action<IReadOnlyDictionary<string, string>>> s_commandRegistry = new Dictionary<string, Action<IReadOnlyDictionary<string, string>>>
        {
            // TODO import these via MEF imports
            { "ManageLaunchProfiles", _ => new LaunchProfilesWindow().ShowDialog() }
        };

        public static ICommand ActionCommand { get; } = new DelegateCommand<Setting>(setting => Handle(setting.EditorMetadata));

        private static void Handle(IReadOnlyDictionary<string, string> editorMetadata)
        {
            // TODO safe dictionary lookups with more helpful error messages

            var action = editorMetadata["Action"];

            switch (action)
            {
                case "Command":
                    var command = editorMetadata["Command"];
                    var handler = s_commandRegistry[command];
                    handler(editorMetadata);
                    break;

                case "URL":
                    var url = editorMetadata["URL"];
                    // TODO sanitize URL for security
                    Process.Start(url);
                    break;
            }
        }
    }
}