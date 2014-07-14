using System;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Search
{
    [ReferenceProviderFactory]
    public class ReflectedMemberReferenceProviderFactory : IReferenceProviderFactory
    {
        public IReferenceFactory CreateFactory(IPsiSourceFile sourceFile, IFile file)
        {
            if (sourceFile.PrimaryPsiLanguage.Is<CSharpLanguage>())
                return new ReflectedMemberReferenceFactory();
            return null;
        }

        public event Action OnChanged;
    }
}
