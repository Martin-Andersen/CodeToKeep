using System;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace SomethingBlue.Validation
{
    public class ValidationTemplate : IDataErrorInfo
    {
        readonly INotifyPropertyChanged _target;

        public ValidationTemplate(INotifyPropertyChanged target)
        {
            _target = target;
            _validator = ValidationFactory.GetValidator(target.GetType());
            _validationResult = _validator.Validate(target);
            target.PropertyChanged += Validate;
        }

        void Validate(object sender, PropertyChangedEventArgs e)
        {
            if (_validator != null)
                _validationResult = _validator.Validate(_target);
        }

        readonly IValidator _validator;
        ValidationResult _validationResult;

        public string Error
        {
            get
            {
                if (_validator == null) return string.Empty;
                var strings = _validationResult.Errors.Select(x => x.ErrorMessage).ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (_validator == null) return string.Empty;
                var strings = _validationResult.Errors.Where(x => x.PropertyName == propertyName).Select(x => x.ErrorMessage).ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }
    }
}