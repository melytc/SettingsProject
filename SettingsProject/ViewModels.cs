using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal sealed class ApplicationViewModel
    {
        public SearchViewModel SearchViewModel { get; }
        public IReadOnlyList<SettingsPageViewModel> Pages { get; }

        public SettingsPageViewModel? SelectedPage { get; set; }

        public ApplicationViewModel(IReadOnlyList<SettingsPageViewModel> pages, SearchViewModel searchViewModel)
        {
            Pages = pages;
            SearchViewModel = searchViewModel;
            SelectedPage = pages.FirstOrDefault();

            searchViewModel.SearchChanged += searchString =>
            {
                foreach (var page in pages)
                {
                    foreach (var setting in page.SettingsListListViewModel.Settings)
                    {
                        setting.IsVisible = setting.MatchesSearchText(searchString);
                    }
                }
            };
        }
    }

    internal sealed class SettingsPageViewModel
    {
        public SettingsListViewModel SettingsListListViewModel { get; }

        public object? ContextControl { get; }

        public string Name { get; }

        public SettingsPageViewModel(string name, SettingsListViewModel settingsListListViewModel, object? contextControl)
        {
            Name = name;
            SettingsListListViewModel = settingsListListViewModel;
            ContextControl = contextControl;
        }
    }

    internal sealed class SettingsListViewModel
    {
        public IReadOnlyList<Setting> Settings { get; }

        public SettingsListViewModel(IReadOnlyList<Setting> settings, SearchViewModel searchViewModel)
        {
            Settings = settings;

            // Construct the default view for our settings collection and customise it.
            // When the view binds the collection, it will use this pre-constructed view.
            // We will be able to use this view for filtering too (search, advanced mode, etc).
            var view = CollectionViewSource.GetDefaultView(Settings);

            if (view is ICollectionViewLiveShaping shaping)
            {
                if (shaping.CanChangeLiveFiltering)
                {
                    shaping.LiveFilteringProperties.Add(nameof(Setting.IsVisible));
                }

                shaping.IsLiveFiltering = true;
            }

            view.Filter = o => o is Setting setting && setting.IsVisible;

            // Specify the property to sort on, and direction to sort.
            view.SortDescriptions.Add(new SortDescription(nameof(Setting.Priority), ListSortDirection.Ascending));

            if (view.CanGroup)
            {
                bool hasMultipleCategories = settings.Select(setting => setting.Category).Distinct().Skip(1).Any();

                if (hasMultipleCategories)
                {
                    view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Category)));
                }
            }
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