using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

#nullable enable

namespace SettingsProject
{
    internal static class VisualTreeUtil
    {
        public static bool TryFindDescendentBreadthFirst<T>(DependencyObject o, [NotNullWhen(returnValue: true)] out T? element)
            where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(o);

            for (var i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(o, i);

                if (child is T dependencyObject)
                {
                    element = dependencyObject;
                    return true;
                }

                if (TryFindDescendentBreadthFirst(child, out element))
                {
                    return true;
                }
            }

            element = null;
            return false;
        }
    }
}