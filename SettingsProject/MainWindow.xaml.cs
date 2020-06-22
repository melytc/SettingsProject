using System.Linq;

#nullable enable

namespace SettingsProject
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var settings = SettingsLoader.DefaultSettings;

            var settingsByPage = settings.ToLookup(setting => setting.Page);

            var pageViewModels = settingsByPage
                .Select(settings => new SettingsPageViewModel(settings.Key, new SettingsListViewModel(settings.ToList())))
                .ToList();

            DataContext = new ApplicationViewModel(pageViewModels, new SearchViewModel());
        }
    }
}
