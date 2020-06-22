using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly Dictionary<string, NavigationPageViewModel> _pageByName;

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

            Pages = categoriesByPage.Select(
                pair => new NavigationPageViewModel(
                    pair.Key,
                    pair.Value.Select(category => new NavigationCategoryViewModel(category)).ToImmutableArray()))
                .ToImmutableArray();

            _pageByName = Pages.ToDictionary(page => page.Name);

            ScrollTo(Pages[0].Name, Pages[0].Categories[0].Name);
        }

        public void ScrollTo(Setting setting)
        {
            ScrollTo(setting.Page, setting.Category);
        }

        private NavigationPageViewModel? _focusedPage;
        private NavigationCategoryViewModel? _focusedCategory;

        private void ScrollTo(string page, string category)
        {
            bool pageChanged = _focusedPage?.Name != page;

            if (pageChanged)
            {
                if (_focusedPage != null)
                {
                    _focusedPage.IsFocused = false;
                }

                if (_pageByName.TryGetValue(page, out _focusedPage))
                {
                    _focusedPage.IsFocused = true;
                }
                else
                {
                    _focusedPage = null;
                    Debug.Fail("Setting page not found.");
                }

                ClearCategory();
            }

            bool categoryChanged = pageChanged || _focusedCategory?.Name != category;

            if (categoryChanged)
            {
                ClearCategory();

                if (_focusedPage != null)
                {
                    _focusedCategory = _focusedPage.Categories.FirstOrDefault(c => c.Name == category);

                    if (_focusedCategory != null)
                    {
                        _focusedCategory.IsFocused = true;
                    }
                }
            }

            void ClearCategory()
            {
                if (_focusedCategory != null)
                {
                    _focusedCategory.IsFocused = false;
                    _focusedCategory = null;
                }
            }
        }
    }

    internal sealed class NavigationPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isFocused;

        public string Name { get; }

        public ImmutableArray<NavigationCategoryViewModel> Categories { get; }

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (_isFocused != value)
                {
                    _isFocused = value;
                    OnPropertyChanged();
                }
            }
        }

        public NavigationPageViewModel(string name, ImmutableArray<NavigationCategoryViewModel> categories)
        {
            Name = name;
            Categories = categories;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal sealed class NavigationCategoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isFocused;

        public string Name { get; }

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (_isFocused != value)
                {
                    _isFocused = value;
                    OnPropertyChanged();
                }
            }
        }

        public NavigationCategoryViewModel(string name)
        {
            Name = name;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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