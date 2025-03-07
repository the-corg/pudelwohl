using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    public class TextClippingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;

            if (value is string)
            {
                MessageBox.Show("String!");
                if (value?.ToString()?.Length < 30)
                    return null;
            }

            if (parameter is ToggleButton toggleButton)
            {
                MessageBox.Show("YES");
                // Get the displayed text (assumes it’s a string, adjust if using complex templates)
                /*string? text = contentPresenter.Content?.ToString();
                if (string.IsNullOrEmpty(text))
                    return null;

                // Get the actual width of the ContentPresenter
                double actualWidth = contentPresenter.ActualWidth;
                double desiredWidth = contentPresenter.DesiredSize.Width;

                // Measure the text size
                var formattedText = new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    16, // ComboBox font size
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(contentPresenter).PixelsPerDip);

                // Show tooltip only if text is clipped
                return desiredWidth > actualWidth;*/
            }

            return null;
        }
        /*public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            string text = value.ToString();
            //double actualWidth = (double)parameter;
            var presenter = value as ContentPresenter;

            // Measure the text size (example using formatted text, adjust based on your method)
            var formattedText = new FormattedText(text,
                                                  CultureInfo.CurrentCulture,
                                                  FlowDirection.LeftToRight,
                                                  new Typeface("Segoe UI"),
                                                  12,
                                                  Brushes.Black,
                                                  VisualTreeHelper.GetDpi(new System.Windows.Controls.TextBlock()).PixelsPerDip);

            // If the text is clipped, return it for the tooltip
            return "TEST";// formattedText.Width > actualWidth ? text : null;
        }*/

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        /*public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4) return null;

            var presenter = values[0] as ContentPresenter;
            var actualWidth = values[1] as double? ?? 0;
            var desiredWidth = values[2] as double? ?? 0;
            var selectedItem = values[3]; // The selected item from ComboBox

            // If the text is clipped, return the selected item as the tooltip
            return actualWidth < desiredWidth ? selectedItem : null;
        }*/
        /*public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ContentPresenter presenter && values[1] is double actualWidth && values[2] is double desiredWidth)
            {
                return actualWidth < desiredWidth ? presenter.Content : null; 
            }
            return null;
        }*/
        /*public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is double actualWidth && values[1] is double desiredWidth && values[2] is string text)
            {
                return desiredWidth > actualWidth ? text : null;  // Show tooltip only if text is clipped
            }
            return null;
        }*/

        /*public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }*/
    }
}
