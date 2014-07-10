using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve.Filters;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;

namespace ReSharper.Reflection.QuickFixes
{
    [QuickFix]
    public class DidYouMeanQuickFix : DidYouMeanQuickFixBase
    {
        private readonly NotResolvedError _error;
        private IAccessContext _accessContext;

        public DidYouMeanQuickFix(NotResolvedError error)
            : base((ICSharpExpression)error.Reference.GetTreeNode(), error.Reference.GetName())
        {
            _error = error;
        }

        protected override ITypeElement GetTypeElement(out bool? isStaticReference)
        {
            isStaticReference = false;
            var referenceNode = _error.Reference.GetTreeNode() as IReferenceExpression;

            if (referenceNode != null && referenceNode.QualifierExpression != null)
            {
                var targetReference = referenceNode.QualifierExpression as IReferenceExpression;

                if (targetReference != null)
                {
                    var completableReference = targetReference.Reference;
                    _accessContext = completableReference.GetAccessContext();

                    var declaredElement = completableReference.Resolve().DeclaredElement;
                    var typeElement = declaredElement as ITypeElement;

                    if (typeElement == null)
                    {
                        typeElement = declaredElement.Type().GetTypeElement<ITypeElement>();
                    }
                    else
                    {
                        isStaticReference = true;
                    }

                    return typeElement;
                }
            }

            return null;
        }

        protected override ICSharpExpression CreateReplacementExpression(ICSharpExpression expression, IDeclaredElement declaredElement)
        {
            CSharpElementFactory factory = CSharpElementFactory.GetInstance(expression);
            return factory.CreateReferenceExpression("$0.$1",
                ((IReferenceExpression)expression).QualifierExpression,
                declaredElement.ShortName);
        }

        protected override void ConfigureFilters(IList<ISymbolFilter> filters)
        {
            base.ConfigureFilters(filters);
            filters.Add(new AccessRightsFilter(_accessContext));
        }
    }
}
