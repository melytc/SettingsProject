using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

#nullable enable

namespace Microsoft.VisualStudio.ProjectSystem.VS.Implementation.PropertyPages.Designer
{
    internal static class PropertyListViewSource
    {
        public static void Initialize(IReadOnlyList<Property> properties, bool useGrouping)
        {
            // Construct the default view for our properties collection and customise it.
            // When the view binds the collection, it will use this pre-constructed view.
            // We will be able to use this view for filtering too (search, advanced mode, etc).
            var view = CollectionViewSource.GetDefaultView(properties);

            if (view is ICollectionViewLiveShaping shaping)
            {
                if (shaping.CanChangeLiveFiltering)
                {
                    shaping.LiveFilteringProperties.Add(nameof(Property.IsVisible));
                }

                shaping.IsLiveFiltering = true;
            }

            view.Filter = o => o is Property property && property.IsVisible;

            // Specify the property to sort on, and direction to sort.
            view.SortDescriptions.Add(new SortDescription(nameof(Property.Priority), ListSortDirection.Ascending));

            if (useGrouping && view.CanGroup)
            {
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Property.Page)));
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Property.Category)));
            }
        }
    }
}