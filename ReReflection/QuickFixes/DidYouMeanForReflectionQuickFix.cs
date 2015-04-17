using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using ReSharper.Reflection.Highlightings;

#if R9
using JetBrains.ReSharper.Feature.Services.QuickFixes;
#else
using JetBrains.ReSharper.Intentions.Extensibility;
#endif

namespace ReSharper.Reflection.QuickFixes
{
    [QuickFix]
    public class DidYouMeanForReflectionQuickFix : DidYouMeanQuickFixBase
    {
        private readonly ReflectionMemberNotFoundError _error;

        public DidYouMeanForReflectionQuickFix(ReflectionMemberNotFoundError error)
            : base((ICSharpExpression)error.NameArgument, error.NameArgumentValue)
        {
            _error = error;
        }

        protected override ITypeElement GetTypeElement(out bool? isStaticReference)
        {
            isStaticReference = null;
            if (_error.BindingFlags.HasValue)
            {
                if ((_error.BindingFlags.Value & BindingFlags.Static) != 0 && (_error.BindingFlags.Value & BindingFlags.Instance) == 0)
                {
                    isStaticReference = true;
                }
                if ((_error.BindingFlags.Value & BindingFlags.Static) == 0 && (_error.BindingFlags.Value & BindingFlags.Instance) != 0)
                {
                    isStaticReference = false;
                }
            }
            return _error.Type;
        }

        protected override ICSharpExpression CreateReplacementExpression(ICSharpExpression expression, IDeclaredElement declaredElement)
        {
            CSharpElementFactory factory = CSharpElementFactory.GetInstance(expression);
            return factory.CreateExpression("\"$0\"", declaredElement.ShortName);
        }

        protected override void ConfigureFilters(IList<ISymbolFilter> filters)
        {
            base.ConfigureFilters(filters);
            filters.Add(new ReflectedMembersSymbolsFilter(_error.ElementType));
        }

        private class ReflectedMembersSymbolsFilter : SimpleSymbolFilter
        {
            private readonly DeclaredElementType _elementType;

            public ReflectedMembersSymbolsFilter(DeclaredElementType elementType)
            {
                _elementType = elementType;
            }

            public override ResolveErrorType ErrorType
            {
                get { return ResolveErrorType.NOT_RESOLVED; }
            }

            public override bool Accepts(IDeclaredElement declaredElement, ISubstitution substitution)
            {
                var method = declaredElement as IMethod;
                if (method != null && method.IsExtensionMethod)
                {
                    return false;
                }

                return _elementType == null || declaredElement.GetElementType() == _elementType;
            }
        }
    }
}
