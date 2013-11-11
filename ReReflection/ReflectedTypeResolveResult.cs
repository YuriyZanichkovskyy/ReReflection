using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReReflection
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

        public ReflectedTypeResolveResult(ITypeElement typeElement, ReflectedTypeResolution resolvedAs)
        {
            TypeElement = typeElement;
            ResolvedAs = resolvedAs;
        }

        public ReflectedTypeResolution ResolvedAs { get; private set; }
    }
}
