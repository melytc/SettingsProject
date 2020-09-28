using System;
using System.Threading.Tasks;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    public partial class ProjectPropertiesWindow
    {
        public ProjectPropertiesWindow()
        {
            DataContext = new AsyncLoadViewModel(Designer.Resources.ProjectPropertiesAsyncLoadMessage);

            InitializeComponent();

            Dispatcher.BeginInvoke(new Func<Task>(
                async () =>
                {
                    // Simulate delayed load
                    await Task.Delay(1000);

                    DataContext = new ProjectPropertiesViewModel(PropertiesLoader.CreateDefaultContext());
                }));
        }
    }
}
