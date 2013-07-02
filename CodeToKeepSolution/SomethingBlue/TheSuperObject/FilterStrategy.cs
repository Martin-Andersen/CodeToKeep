using System.Collections.Generic;

namespace SomethingBlue.TheSuperObject
{

   public class PropertyFilterManager
    {
        public PropertyFilterManager(string className, List<string> propertyNames, bool filterIsPositivList = true)
        {
            ClassName = className;
            IsPositivList = filterIsPositivList;
            PropertyNames = propertyNames;
        }

        public string ClassName { get; set; }
        public bool IsPositivList { get; private set; }
        public List<string> PropertyNames { get; set; }
    }

    public class MultiSelectStrategy
    {
        public MultiSelectStrategy(string propertyName, StrategyType strategy = StrategyType.Show, object defaultValue = null)
        {
            PropertyName = propertyName;
            Strategy = strategy;
            DefaultValue = defaultValue;
        }

        public MultiSelectStrategy(string propertyName, object defaultValue): this(propertyName, StrategyType.Show, defaultValue)
        {
        }

        public string PropertyName { get; set; }
        public StrategyType Strategy { get; set; }
        public object DefaultValue { get; set; }
    }

    public enum StrategyType
    {
        Show = 0,
        ShowAndValidate,
        Hide,
    }
}