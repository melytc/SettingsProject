using System;
using System.Threading.Tasks;

#nullable enable

namespace SettingsProject
{
    internal partial class LaunchProfilesWindow
    {
        public LaunchProfilesWindow()
        {
            DataContext = new AsyncLoadViewModel("Loading launch profiles...");

            InitializeComponent();

            Dispatcher.BeginInvoke(new Func<Task>(
                async () =>
                {
                    // Simulate delayed load
                    await Task.Delay(3000);

                    DataContext = SettingsLoader.CreateLaunchProfiles();
                }));
        }
    }
}
