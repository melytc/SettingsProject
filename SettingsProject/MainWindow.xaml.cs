#nullable enable

namespace SettingsProject
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var searchViewModel = new SearchViewModel();
            
            var settingsListViewModel = new SettingsViewModel(SettingsLoader.DefaultSettings, searchViewModel);

            DataContext = new ApplicationViewModel(settingsListViewModel, searchViewModel);
        }
    }
}
