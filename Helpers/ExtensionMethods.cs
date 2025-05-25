using System.Windows.Media;
using System.Windows;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    /// <summary>
    /// Contains extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Searches through the visual tree recursively and returns the first found child of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the child that has to be found</typeparam>
        /// <param name="dependencyObject">The parent object starting from which the visual tree is searched recursively</param>
        /// <returns>The first found child of type <typeparamref name="T"/>, or <c>null</c>, if such a child doesn't exist</returns>
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
