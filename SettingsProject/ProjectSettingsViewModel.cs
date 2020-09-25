using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class ProjectSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _searchText = "";

        public IReadOnlyList<Setting> Settings { get; }
        public NavigationViewModel NavigationViewModel { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                var searchText = value.Trim();

                if (!string.Equals(_searchText, searchText, StringComparisons.SearchText))
                {
                    _searchText = searchText;

                    foreach (var setting in Settings)
                    {
                        setting.UpdateSearchState(searchText);
                    }

                    OnPropertyChanged();
                }
            }
        }

        public ProjectSettingsViewModel(SettingContext context)
        {
            Settings = context.Settings;

            SettingsListViewSource.Initialize(Settings, useGrouping: true);

            NavigationViewModel = new NavigationViewModel(Settings);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}