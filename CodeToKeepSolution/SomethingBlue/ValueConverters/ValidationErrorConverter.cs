using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace SomethingBlue.ValueConverters
{
    /// <summary> 
    /// Helps with avoiding of exceptions when setting a tooltip to the validation error 
    /// http://www.jimandkatrin.com/CodeBlog/post/WPF-Validation-Exceptions.aspx
    /// </summary> 
    /// <remarks> 
    /// Bind to Path=(Validation.Errors).CurrentItem, and use this converter 
    /// </remarks> 
    public class ValidationErrorConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var error = value as ValidationError;
            return error != null ? error.ErrorContent : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Can't convert back to a ValidationError 
            throw new Exception("The method or operation is not implemented.");
        }
    }
}