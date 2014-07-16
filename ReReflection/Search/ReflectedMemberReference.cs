using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.LinqTools;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReSharper.Reflection.Search
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
            if (GetName() != element.ShortName)
            {
                CSharpElementFactory instance = CSharpElementFactory.GetInstance(myOwner, true);
                if (_resolveResult.ResolveErrorType != ResolveErrorType.OK)
                {
                    var elementNameExpression = myOwner.ReplaceBy(instance.CreateExpression("\"$0\"", element.ShortName));
                    return new ReflectedMemberReference(elementNameExpression,
                        new ResolveResultWithInfo(ResolveResultFactory.CreateResolveResult(element), ResolveErrorType.OK),
                        _typeElement);               
                }
                else if (_resolveResult.ResolveErrorType != ResolveErrorType.MULTIPLE_CANDIDATES)
                {

                }
            }
            return this;
        }

        public override IReference BindTo(IDeclaredElement element, ISubstitution substitution)
        {
            return BindTo(element);
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
