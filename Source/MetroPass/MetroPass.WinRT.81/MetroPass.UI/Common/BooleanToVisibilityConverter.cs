using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MetroPass.UI.Common
{
    /// <summary>
    /// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
    /// <see cref="Visibility.Collapsed"/>.
    /// </summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool b = (value is bool && (bool)value);
            bool parm = false;

            if (parameter != null && Boolean.TryParse(parameter.ToString(), out parm) && parm)
                b = !b;

            Visibility visibility = b ? Visibility.Visible : Visibility.Collapsed;
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
