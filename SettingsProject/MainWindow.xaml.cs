#nullable enable

namespace SettingsProject
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new ApplicationViewModel();

            InitializeComponent();
        }
    }
}
