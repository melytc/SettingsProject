using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class ProjectPropertiesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _searchText = "";

        public IReadOnlyList<Property> Properties { get; }
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

                    foreach (var property in Properties)
                    {
                        property.UpdateSearchState(searchText);
                    }

                    OnPropertyChanged();
                }
            }
        }

        public ProjectPropertiesViewModel(PropertyContext context)
        {
            Properties = context.Properties;

            PropertyListViewSource.Initialize(Properties, useGrouping: true);

            NavigationViewModel = new NavigationViewModel(Properties);
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}