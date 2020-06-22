using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal sealed class ApplicationViewModel
    {
        public SettingsListViewModel SettingsListViewModel { get; }
        public SearchViewModel SearchViewModel { get; }
        public NavigationViewModel NavigationViewModel { get; }

        public ApplicationViewModel()
        {
            var settings = SettingsLoader.DefaultSettings;

            SettingsListViewModel = new SettingsListViewModel(settings);

            NavigationViewModel = new NavigationViewModel(settings);

            SearchViewModel = new SearchViewModel();
            
            SearchViewModel.SearchChanged += searchString =>
            {
                foreach (var setting in settings)
                {
                    setting.IsVisible = setting.MatchesSearchText(searchString);
                }
            };
        }
    }

    internal sealed class NavigationViewModel
    {
        public ImmutableArray<NavigationPageViewModel> Pages { get; }

        public NavigationViewModel(IReadOnlyList<Setting> settings)
        {
            var categoriesByPage = new Dictionary<string, HashSet<string>>();

            foreach (var setting in settings)
            {
                if (!categoriesByPage.TryGetValue(setting.Page, out var categories))
                {
                    categories = categoriesByPage[setting.Page] = new HashSet<string>();
                }

                categories.Add(setting.Category);
            }

            Pages = categoriesByPage.Select(pair => new NavigationPageViewModel(pair.Key, pair.Value.ToImmutableArray())).ToImmutableArray();

            Pages[0].IsFocused = true;
        }
    }

    internal sealed class NavigationPageViewModel
    {
        public string Name { get; }

        public ImmutableArray<string> Categories { get; }

        public bool IsFocused { get; set; }

        public NavigationPageViewModel(string name, ImmutableArray<string> categories)
        {
            Name = name;
            Categories = categories;
        }
    }

    internal sealed class SettingsListViewModel
    {
        public IReadOnlyList<Setting> Settings { get; }

        public SettingsListViewModel(IReadOnlyList<Setting> settings)
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
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Page)));
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Category)));
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