using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

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
        public IType Type { get; private set; }

        public static readonly ReflectedTypeResolveResult NotResolved = new ReflectedTypeResolveResult();

        private ReflectedTypeResolveResult()
        {
            ResolvedAs = ReflectedTypeResolution.NotResolved;
        }

        public ReflectedTypeResolveResult([NotNull] IType type, ReflectedTypeResolution resolvedAs)
        {
            if (type == null) throw new ArgumentNullException("type");
            TypeElement = type.GetTypeElement<ITypeElement>();
            Type = type;
            ResolvedAs = resolvedAs;
        }

        public ReflectedTypeResolution ResolvedAs { get; private set; }
    }
}
