using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

#nullable enable

namespace SettingsProject
{
    internal static class SettingsListViewSource
    {
        public static void Initialize(IReadOnlyList<Setting> settings, bool useGrouping)
        {
            // Construct the default view for our settings collection and customise it.
            // When the view binds the collection, it will use this pre-constructed view.
            // We will be able to use this view for filtering too (search, advanced mode, etc).
            var view = CollectionViewSource.GetDefaultView(settings);

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

            if (useGrouping && view.CanGroup)
            {
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Page)));
                view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Setting.Category)));
            }
        }
    }
}