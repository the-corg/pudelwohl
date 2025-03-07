using System.Windows.Media;
using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    public static class ExtensionMethods
    {
        public static T? GetVisualChildOfType<T>(this DependencyObject? dependencyObject)
        where T : DependencyObject
        {
            if (dependencyObject is null)
                return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                var result = (child as T) ?? GetVisualChildOfType<T>(child);

                if (result is not null)
                    return result;
            }
            return null;
        }
    }
}
