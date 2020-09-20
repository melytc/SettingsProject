#nullable enable

namespace SettingsProject
{
    internal partial class LaunchProfilesWindow
    {
        public LaunchProfilesWindow()
        {
            DataContext = SettingsLoader.CreateLaunchProfiles();

            InitializeComponent();
        }
    }
}
