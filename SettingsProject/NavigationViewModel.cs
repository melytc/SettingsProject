﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal sealed class NavigationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Dictionary<string, NavigationPageViewModel> _pageByName;

        private NavigationPageViewModel? _focusedPage;
        private NavigationCategoryViewModel? _focusedCategory;
        private NavigationSection _selectedSection;

        public ImmutableArray<NavigationPageViewModel> Pages { get; }

        public NavigationSection SelectedSection
        {
            get => _selectedSection;
            set
            {
                if (_selectedSection != value)
                {
                    _selectedSection = value;
                    ScrollTo(value.Page, value.Category);
                    OnPropertyChanged();
                }
            }
        }

        public NavigationViewModel(IReadOnlyList<Property> properties)
        {
            var categoriesByPage = new Dictionary<string, HashSet<string>>();

            foreach (var property in properties)
            {
                if (!categoriesByPage.TryGetValue(property.Page, out var categories))
                {
                    categories = categoriesByPage[property.Page] = new HashSet<string>();
                }

                categories.Add(property.Category);
            }

            Pages = categoriesByPage.Select(
                pair => new NavigationPageViewModel(
                    pair.Key,
                    pair.Value.Select(category => new NavigationCategoryViewModel(pair.Key, category)).ToImmutableArray()))
                .ToImmutableArray();

            _pageByName = Pages.ToDictionary(page => page.Name);

            if (Pages.Length != 0 && Pages[0].Categories.Length != 0)
            {
                ScrollTo(Pages[0].Name, Pages[0].Categories[0].CategoryName);
            }
        }

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
                    Debug.Fail("Property page not found.");
                }

                ClearCategory();
            }

            bool categoryChanged = pageChanged || _focusedCategory?.CategoryName != category;

            if (categoryChanged)
            {
                ClearCategory();

                if (_focusedPage != null)
                {
                    _focusedCategory = _focusedPage.Categories.FirstOrDefault(c => c.CategoryName == category);

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

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public string PageName { get; }
        public string CategoryName { get; }

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

        public NavigationCategoryViewModel(string pageName, string categoryName)
        {
            CategoryName = categoryName;
            PageName = pageName;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}