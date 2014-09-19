using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.Validations
{
    public class GetEnumXMethodValidator : ReflectionTypeMethodValidatorBase
    {
        public GetEnumXMethodValidator(IMethod reflectionMethod) 
            : base(reflectionMethod)
        {
        }

        public override ReflectionHighlightingBase Validate(ReflectedTypeResolveResult resolvedType, IInvocationExpression invocation)
        {
            if (resolvedType.TypeElement.GetElementType() != CLRDeclaredElementType.ENUM)
            {
                return new IsNotEnumTypeError(invocation);
            }

            return null;
        }
    }
}
