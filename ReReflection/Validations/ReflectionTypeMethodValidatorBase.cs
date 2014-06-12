using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.Validations
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
