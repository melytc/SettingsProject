#nullable enable

namespace SettingsProject
{
    internal readonly struct NavigationSection
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

        public override string ToString() => $"{Page} | {Category}";
    }
}