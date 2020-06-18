namespace SettingsProject
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new SettingsViewModel();
        }
    }
}
