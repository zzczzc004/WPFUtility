using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfUtility.Converters
{
    /// <summary>
    /// 多个条件
    /// </summary>
    public class BoolMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var value in values)
            {
                if (value is bool && !(bool)value)
                {
                    return false;
                }
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
