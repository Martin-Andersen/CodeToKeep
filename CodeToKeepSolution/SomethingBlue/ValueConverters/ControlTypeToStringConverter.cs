using System;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Data;

namespace SomethingBlue.ValueConverters
{
    public class ControlTypeToStringConverter : BaseConverter, IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = Enum.GetName(typeof (ControlType), value);
            return type;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
