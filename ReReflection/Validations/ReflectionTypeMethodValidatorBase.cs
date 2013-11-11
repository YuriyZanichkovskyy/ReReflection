using System;
using System.Linq;
using System.Reflection;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReReflection.Highlightings;

namespace ReReflection.Validations
{
    public abstract class ReflectionTypeMethodValidatorBase
    {
        public IMethod ReflectionMethod { get; private set; }
        protected readonly static System.Type _T = null;
        
        protected ReflectionTypeMethodValidatorBase(IMethod reflectionMethod)
        {
            ReflectionMethod = reflectionMethod;
        }

        public virtual bool CanValidate(ReflectedTypeResolveResult reflectedType)
        {
            return reflectedType.ResolvedAs != ReflectedTypeResolution.NotResolved;
        }

        public abstract ReflectionHighlightingBase Validate(ReflectedTypeResolveResult resolvedType, IInvocationExpression invocation);
    }
}
