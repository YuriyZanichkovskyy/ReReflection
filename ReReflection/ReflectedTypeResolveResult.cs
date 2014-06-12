using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection
{
    public enum ReflectedTypeResolution
    {
        NotResolved,
        Exact,
        ExactMakeGeneric,
        BaseClass
    }

    public class ReflectedTypeResolveResult
    {
        public ITypeElement TypeElement { get; private set; }
        public static readonly ReflectedTypeResolveResult NotResolved = new ReflectedTypeResolveResult();

        private ReflectedTypeResolveResult()
        {
            ResolvedAs = ReflectedTypeResolution.NotResolved;
        }

        public ReflectedTypeResolveResult([NotNull] ITypeElement typeElement, ReflectedTypeResolution resolvedAs)
        {
            if (typeElement == null) throw new ArgumentNullException("typeElement");
            TypeElement = typeElement;
            ResolvedAs = resolvedAs;
        }

        public ReflectedTypeResolution ResolvedAs { get; private set; }
    }
}
