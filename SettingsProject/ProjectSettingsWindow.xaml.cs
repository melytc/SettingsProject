using System;
using System.Threading.Tasks;

#nullable enable

namespace SettingsProject
{
    public partial class ProjectSettingsWindow
    {
        public ProjectSettingsWindow()
        {
            DataContext = new AsyncLoadViewModel(SettingsProject.Resources.ProjectSettingsAsyncLoadMessage);

            InitializeComponent();

            Dispatcher.BeginInvoke(new Func<Task>(
                async () =>
                {
                    // Simulate delayed load
                    await Task.Delay(3000);

                    DataContext = new ProjectSettingsViewModel(SettingsLoader.CreateDefaultContext());
                }));
        }
    }
}
