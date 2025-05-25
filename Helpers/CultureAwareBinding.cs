using System.Globalization;
using System.Windows.Data;

namespace Pudelwohl_Hotel_and_Resort_Management_Suite_Ultimate_Wuff_Wuff.Helpers
{
    /// <summary>
    /// Binding that explicitly uses the current culture
    /// (without this, the short format date was always using the US date format)
    /// </summary>
    public class CultureAwareBinding : Binding
    {
        public CultureAwareBinding()
        {
            ConverterCulture = CultureInfo.CurrentCulture;
        }
    }
}
