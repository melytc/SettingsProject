using System.Collections.Immutable;
using System.Windows;
using System.Windows.Input;

#nullable enable

namespace SettingsProject
{
    internal struct NavigationSection
    {
        public string Page { get; }
        public string Category { get; }

        public NavigationSection(string page, string category)
        {
            Page = page;
            Category = category;
        }

        public bool Equals(NavigationSection other) => Page == other.Page && Category == other.Category;
        public override bool Equals(object? obj) => obj is NavigationSection other && Equals(other);
        public override int GetHashCode() => unchecked(Page.GetHashCode() * 397) ^ Category.GetHashCode();
        public static bool operator ==(NavigationSection left, NavigationSection right) => left.Equals(right);
        public static bool operator !=(NavigationSection left, NavigationSection right) => !left.Equals(right);
    }

    internal partial class NavigationTree
    {
        public static readonly DependencyProperty PagesProperty = DependencyProperty.Register(
            nameof(Pages),
            typeof(ImmutableArray<NavigationPageViewModel>),
            typeof(NavigationTree),
            new PropertyMetadata(ImmutableArray<NavigationPageViewModel>.Empty));

        public static readonly DependencyProperty SelectedSectionProperty = DependencyProperty.Register(
            nameof(SelectedSection),
            typeof(NavigationSection),
            typeof(NavigationTree),
            new PropertyMetadata(default(NavigationSection)));

        public NavigationTree()
        {
            InitializeComponent();
        }

        public ImmutableArray<NavigationPageViewModel> Pages
        {
            get => (ImmutableArray<NavigationPageViewModel>)GetValue(PagesProperty);
            set => SetValue(PagesProperty, value);
        }

        public NavigationSection SelectedSection
        {
            get => (NavigationSection)GetValue(SelectedSectionProperty);
            set => SetValue(SelectedSectionProperty, value);
        }

        private void OnNavigationPageLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement element)
            {
                if (element.DataContext is NavigationPageViewModel model)
                {
                    if (model.Categories.Length != 0)
                    {
                        SelectedSection = new NavigationSection(model.Name, model.Categories[0].CategoryName);
                    }
                }
            }
        }

        private void OnNavigationCategoryLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is FrameworkElement element)
            {
                if (element.DataContext is NavigationCategoryViewModel model)
                {
                    SelectedSection = new NavigationSection(model.PageName, model.CategoryName);
                }
            }
        }
    }
}
