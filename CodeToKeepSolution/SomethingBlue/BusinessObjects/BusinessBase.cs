using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation;
using PropertyChanged;

namespace SomethingBlue.BusinessObjects
{
    /// <summary>
    /// Base class for business objects
    /// FluentValidation is used for validation
    /// Fody is used to auto implement INotifyPropertyChanged  
    /// </summary>
    public abstract class BusinessBase : INotifyPropertyChanged, IDataErrorInfo
    {
        [Browsable(false)]
        [field: NonSerialized]
#pragma warning disable 169
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 169

        [Browsable(false)]
        public bool IsChanged { get; set; }

        [Browsable(false)]
        [DoNotSetChanged]
        public IValidator Validator { get; set; }

        [Browsable(false)]
        [DoNotSetChanged]
        public bool IsValid { get; set; }

        [Browsable(false)]
        public void Validate()
        {
            // force validation
            // ReSharper disable once UnusedVariable
            var error = Error;
        }

        [Browsable(false)]
        [DoNotSetChanged]
        public string Error
        {
            get
            {
                if (Validator == null)
                {
                    IsValid = true;
                    return string.Empty;
                }
                var result = Validator.Validate(this);
                var strings = result.Errors.Select(x => x.ErrorMessage).ToArray();
                IsValid = result.IsValid;
                return string.Join(Environment.NewLine, strings);
            }
        }

        [Browsable(false)]
        [DoNotSetChanged]
        public string this[string propertyName]
        {
            get
            {
                if (Validator == null)
                {
                    IsValid = true;
                    return string.Empty;
                }
                var result = Validator.Validate(this);
                var strings = result.Errors.Where(x => x.PropertyName == propertyName).Select(x => x.ErrorMessage).ToArray();
                IsValid = result.IsValid;
                return string.Join(Environment.NewLine, strings);
            }
        }
        [Browsable(false)]
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}