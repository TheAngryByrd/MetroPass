using System;
using Windows.UI.Xaml.Data;

namespace MetroPass.UI.Common.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // No format provided.
            if (parameter == null)
            {
                return value;
            }
            var retVal = string.Format((String)parameter,value);

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
