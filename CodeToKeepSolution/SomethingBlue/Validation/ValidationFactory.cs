using System;
using FluentValidation;

namespace SomethingBlue.Validation
{
    public class ValidationFactory 
    {
        public static IValidator GetValidator(object model)
        {
            return GetValidator(model.GetType());
        }

        public static IValidator GetValidator(Type modelType)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(modelType);
            //return (IValidator)RxApp.GetService(validatorType);
            return null;
        }
    }
}