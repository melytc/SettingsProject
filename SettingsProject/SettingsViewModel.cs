using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal sealed class ApplicationViewModel
    {
        public SettingsViewModel SettingsListListViewModel { get; }
        public SearchViewModel SearchViewModel { get; }

        public ApplicationViewModel(SettingsViewModel settingsListListViewModel, SearchViewModel searchViewModel)
        {
            SettingsListListViewModel = settingsListListViewModel;
            SearchViewModel = searchViewModel;
        }
    }

    internal sealed class SettingsViewModel
    {
        public IReadOnlyList<Setting> Settings { get; }

        public SettingsViewModel(IReadOnlyList<Setting> settings, SearchViewModel searchViewModel)
        {
            Settings = settings;

            // Construct the default view for our settings collection and customise it.
            // When the view binds the collection, it will use this pre-constructed view.
            // We will be able to use this view for filtering too (search, advanced mode, etc).
            var view = CollectionViewSource.GetDefaultView(Settings);

            // Specify the property to sort on, and direction to sort.
            view.SortDescriptions.Add(new SortDescription(nameof(Setting.Priority), ListSortDirection.Ascending));

            if (view.CanGroup)
            {
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Category)));
            }

            searchViewModel.SearchChanged += searchString =>
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    view.Filter = null;
                }
                else
                {
                    view.Filter = o => o is Setting setting && setting.MatchesSearchText(searchString);
                }
            };
        }
    }

    internal sealed class SearchViewModel
    {
        public Action<string>? SearchChanged;

        private string _searchString = "";

        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString != value)
                {
                    _searchString = value;
                    SearchChanged?.Invoke(value.Trim());
                }
            }
        }
    }
}