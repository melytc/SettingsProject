using System;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal partial class LaunchProfilesWindow
    {
        public LaunchProfilesWindow()
        {
            DataContext = new AsyncLoadViewModel(Designer.Resources.LaunchProfilesWindowAsyncLoadMessage);

            InitializeComponent();

            Dispatcher.BeginInvoke(new Func<Task>(
                async () =>
                {
                    // Simulate delayed load
                    await Task.Delay(1000);

                    DataContext = SettingsLoader.CreateLaunchProfiles();
                }));
        }
    }
}
