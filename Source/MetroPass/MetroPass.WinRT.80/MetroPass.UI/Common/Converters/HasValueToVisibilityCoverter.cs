using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MetroPass.UI.Common.Converters
{
    public class HasValueToVisibilityCoverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            var defaultValue = GetDefaultValue(value.GetType());

            return (value.Equals( defaultValue)) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object GetDefaultValue(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        } 

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // TODO: Implement this method
            return value;
        }
    }
}
