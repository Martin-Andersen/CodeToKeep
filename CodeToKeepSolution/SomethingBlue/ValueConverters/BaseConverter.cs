using System;
using System.Windows.Markup;

namespace SomethingBlue.ValueConverters
{
    public class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}