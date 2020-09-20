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
            if (!editorMetadata.TryGetValue("Action", out string action))
                throw new Exception("LinkAction setting must contain \"Action\" metadata.");

            switch (action)
            {
                case "Command":
                    if (!editorMetadata.TryGetValue("Command", out string command))
                        throw new Exception("LinkAction of type \"Command\" must contain \"Command\" metadata.");
                    if (!s_commandRegistry.TryGetValue(command, out var handler))
                        throw new Exception($"Unknown LinkAction command identifier \"{command}\".");
                    handler(editorMetadata);
                    break;

                case "URL":
                    if (!editorMetadata.TryGetValue("URL", out string url))
                        throw new Exception("LinkAction of type \"URL\" must contain \"URL\" metadata.");
                    if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                        throw new Exception("LinkAction of type \"URL\" must contain \"URL\" metadata.");
                    if (uri.Scheme != "http" && uri.Scheme != "https")
                        throw new Exception("LinkAction URL must have http or https scheme.");
                    Process.Start(uri.ToString());
                    break;
            }
        }
    }
}