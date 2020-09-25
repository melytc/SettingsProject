#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal readonly struct PropertyIdentity
    {
        public string Page { get; }
        public string Category { get; }
        public string Name { get; }

        public PropertyIdentity(string page, string category, string name)
        {
            Page = page;
            Category = category;
            Name = name;
        }

        public bool Equals(PropertyIdentity other) => Page == other.Page && Category == other.Category && Name == other.Name;
        public override bool Equals(object? obj) => obj is PropertyIdentity other && Equals(other);
        public static bool operator ==(PropertyIdentity left, PropertyIdentity right) => left.Equals(right);
        public static bool operator !=(PropertyIdentity left, PropertyIdentity right) => !left.Equals(right);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Page.GetHashCode();
                hashCode = (hashCode * 397) ^ Category.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"{Page} | {Category} | {Name}";
    }
}