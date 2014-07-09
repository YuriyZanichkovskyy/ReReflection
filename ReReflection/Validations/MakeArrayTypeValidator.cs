using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.Validations
{
    public class MakeArrayTypeValidator : ReflectionTypeMethodValidatorBase
    {
        public MakeArrayTypeValidator(IMethod reflectionMethod) : base(reflectionMethod)
        {
        }

        public override ReflectionHighlightingBase Validate(ReflectedTypeResolveResult resolvedType, IInvocationExpression invocation)
        {
            if (((IModifiersOwner)resolvedType.TypeElement).IsStatic)
            {
                return new IncorrectMakeArrayTypeHighlighting(invocation, "Could not call MakeArrayType on static type.");
            }

            return null;
        }
    }
}
