using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Converters
{
    public class TextClippingConverter : IValueConverter
    {
        // Makes ToolTip visible if the selected item does not fit completely in the combobox's ContentPresenter
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null)
                return Visibility.Collapsed;

            if (value is ToggleButton)
            {
                var combobox = (value as ToggleButton)?.TemplatedParent as ComboBox;
                var contentPresenter = combobox.GetVisualChildOfType<ContentPresenter>();
                var textblock = contentPresenter.GetVisualChildOfType<TextBlock>();
                if (textblock is not null)
                {
                    // if the text in the TextBlock is longer than the width of the container where it is being shown,
                    // then display the tooltip with the full text. Hide the tooltip otherwise.
                    return (textblock.ActualWidth) > (contentPresenter!.ActualWidth + 2) ? 
                        Visibility.Visible : Visibility.Collapsed;
                }   
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
