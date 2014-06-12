using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.ReSharper.Psi;
using ReSharper.Reflection.Validations;

namespace ReSharper.Reflection
{
    public static class ReflectionValidatorsRegistry
    {
        private static readonly Type _T = typeof (object);
        private static readonly IDictionary<string, Func<IMethod, ReflectionTypeMethodValidatorBase>> _registeredValidators = 
            new Dictionary<string, Func<IMethod, ReflectionTypeMethodValidatorBase>>
            {
                //MakeGenericType
                { Methods.Of<Func<Type[], Type>>(() => _T.MakeGenericType).XmlDocId(), (m) => new MakeGenericTypeValidator(m) },

                //GetProperty overloads
                { Methods.Of<Func<string, PropertyInfo>>(() => _T.GetProperty).XmlDocId(), (m) => new GetPropertyMethodValidator(m) },
                { Methods.Of<Func<string, BindingFlags, PropertyInfo>>(() => _T.GetProperty).XmlDocId(), (m) => new GetPropertyMethodValidator(m, 1) },
                //GetField
                { Methods.Of<Func<string, FieldInfo>>(() => _T.GetField).XmlDocId(), (m) => new GetFieldMethodValidator(m) },
                { Methods.Of<Func<string, BindingFlags, FieldInfo>>(() => _T.GetField).XmlDocId(), (m) => new GetFieldMethodValidator(m, 1) },
                //GetMethod
                { Methods.Of<Func<string, MethodInfo>>(() => _T.GetMethod).XmlDocId(), (m) => new GetMethodMethodValidator(m) },
                { Methods.Of<Func<string, BindingFlags, MethodInfo>>(() => _T.GetMethod).XmlDocId(), (m) => new GetMethodMethodValidator(m, 1) }
            };

        public static ReflectionTypeMethodValidatorBase GetValidator(IMethod method)
        {           
            Func<IMethod, ReflectionTypeMethodValidatorBase> validatorFactory;
            if (_registeredValidators.TryGetValue(method.XMLDocId, out validatorFactory))
            {
                return validatorFactory(method);
            }

            return null;
        }
    }
}
