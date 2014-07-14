using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Finder;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReSharper.Reflection.ReferenceProviders
{
    public class ReflectedMemberReference : TreeReferenceBase<IExpression>, IAccessContext
    {
        private readonly ResolveResultWithInfo _resolveResult;
        private readonly ITypeElement _typeElement;
        public static readonly Key<ReflectedMemberReference> Key = new Key<ReflectedMemberReference>("ReflectedMemberReference");

        public ReflectedMemberReference([NotNull] IExpression owner, ResolveResultWithInfo resolveResult, ITypeElement typeElement) 
            : base(owner)
        {
            _resolveResult = resolveResult;
            _typeElement = typeElement;
        }

        public override ResolveResultWithInfo ResolveWithoutCache()
        {
            return _resolveResult;
        }

        public override string GetName()
        {
            return myOwner.IsConstantValue() && myOwner.ConstantValue.Value != null
                ? myOwner.ConstantValue.Value.ToString()
                : string.Empty;
        }

        public override ISymbolTable GetReferenceSymbolTable(bool useReferenceName)
        {
            throw new NotImplementedException();
        }

        public override TreeTextRange GetTreeTextRange()
        {
            return myOwner.GetTreeTextRange();
        }

        public override IReference BindTo(IDeclaredElement element)
        {
            return this;
        }

        public override IReference BindTo(IDeclaredElement element, ISubstitution substitution)
        {
            return this;
        }

        public override IAccessContext GetAccessContext()
        {
            return this;
        }

        public ITypeElement GetAccessContainingTypeElement()
        {
            return null;
        }

        public Staticness GetStaticness()
        {
            return Staticness.Any;
        }

        public QualifierKind GetQualifierKind()
        {
            return QualifierKind.NONE;
        }

        public ITypeElement GetQualifierTypeElement()
        {
            return null;
        }

        public IPsiModule GetPsiModule()
        {
            return myOwner.GetPsiModule();
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
