#nullable enable

namespace SettingsProject
{
    public partial class MainWindow
    {
        private readonly ApplicationViewModel _applicationViewModel;

        public MainWindow()
        {
            DataContext = _applicationViewModel = new ApplicationViewModel();

            InitializeComponent();
        }

        private void OnScrolledSettingChanged(Setting setting)
        {
            _applicationViewModel.NavigationViewModel.ScrollTo(setting);
        }
    }
}
