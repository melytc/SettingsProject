using System.Linq;

#nullable enable

namespace SettingsProject
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var searchViewModel = new SearchViewModel();

            var settings = SettingsLoader.DefaultSettings;

            var settingsByPage = settings.ToLookup(setting => setting.Page);

            // TODO populate context objects

            var pageViewModels = settingsByPage
                .Select(settings => new SettingsPageViewModel(settings.Key, new SettingsListViewModel(settings.ToList(), searchViewModel), null))
                .ToList();

            DataContext = new ApplicationViewModel(pageViewModels, searchViewModel);
        }
    }
}
