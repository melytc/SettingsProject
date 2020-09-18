#nullable enable

namespace SettingsProject
{
    public partial class ProjectSettingsWindow
    {
        public ProjectSettingsWindow()
        {
            DataContext = new ApplicationViewModel();

            InitializeComponent();
        }
    }
}
