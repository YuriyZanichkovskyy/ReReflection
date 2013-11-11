using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.Resolve.Filters;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve.Filters;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReReflection
{
    public class ReplaceWithSimilarName : BulbActionBase
    {
        private readonly IReferenceExpression _referenceExpression;
        private readonly IDeclaredElement _element;
        private readonly PsiLanguageType _language;

        public ReplaceWithSimilarName(IReferenceExpression referenceExpression, IDeclaredElement element, PsiLanguageType language)
        {
            _referenceExpression = referenceExpression;
            _element = element;
            _language = language;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            CSharpElementFactory factory = CSharpElementFactory.GetInstance(_referenceExpression);
            _referenceExpression.ReplaceBy(factory.CreateReferenceExpression("$0.$1",
                _referenceExpression.QualifierExpression,
                _element.ShortName));
            return null;
        }

        public override string Text
        {
            get
            {
                return string.Format("Did you mean {0} '{1}'?", 
                    DeclaredElementPresenter.Format(_language, DeclaredElementPresenter.KIND_PRESENTER, _element),
                    DeclaredElementPresenter.Format(_language, DeclaredElementPresenter.NAME_PRESENTER, _element));
            }
        }
    }


    [QuickFix]
    public class DidYouMeanQuickFix : IQuickFix
    {
        private readonly NotResolvedError _error;
        private const int _threshold = 2;

        public DidYouMeanQuickFix(NotResolvedError error)
        {
            _error = error;
        }

        public IEnumerable<IntentionAction> CreateBulbItems()
        {
            return GetMostSimilarNamesBulbActions().ToQuickFixAction();
        }

        private IEnumerable<ReplaceWithSimilarName> GetMostSimilarNamesBulbActions()
        {
            var referenceNode = _error.Reference.GetTreeNode() as IReferenceExpression;

            if (referenceNode != null && referenceNode.QualifierExpression != null)
            {
                var language = referenceNode.Language;
                string referenceName = _error.Reference.GetName();
                var targetReference = referenceNode.QualifierExpression as IReferenceExpression;

                if (targetReference != null)
                {
                    var completableReference = targetReference.Reference;

                    var declaredElement = completableReference.Resolve().DeclaredElement;
                    var typeElement = declaredElement as ITypeElement;
                    bool isStaticReference = false;

                    if (typeElement == null)
                    {
                        typeElement = declaredElement.Type().GetTypeElement<ITypeElement>();
                    }
                    else
                    {
                        isStaticReference = true;
                    }

                    var symbolFilters = new ISymbolFilter[]
                                    {
                                        new AccessRightsFilter(completableReference.GetAccessContext()), 
                                        new StaticOrInstanceFilter(isStaticReference)
                                    };

                    var symbolTable = ResolveUtil.GetSymbolTableByTypeElement(typeElement, SymbolTableMode.FULL, referenceNode.GetPsiModule());
                    symbolTable = symbolTable.Filter(symbolFilters);

                    IList<ReplaceWithSimilarName> result = new List<ReplaceWithSimilarName>();
                    symbolTable.ForAllSymbolInfos(s =>
                    {
                        if (CalculateLevenshteinDistance(referenceName, s.GetDeclaredElement().ShortName) <= _threshold)
                        {
                            result.Add(new ReplaceWithSimilarName(referenceNode, s.GetDeclaredElement(), language));
                        }
                    });

                    return result;
                }
            }

            return Enumerable.Empty<ReplaceWithSimilarName>();
        }

        public bool IsAvailable(IUserDataHolder cache)
        {
            return true;
        }

        private int CalculateLevenshteinDistance(string a, string b)
        {
            int[,] res = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i < a.Length + 1; i++)
            {
                res[i, 0] = i;
            }

            for (int i = 0; i < b.Length + 1; i++)
            {
                res[0, i] = i;
            }

            for (int i = 1; i < a.Length + 1; i++)
            {
                for (int j = 1; j < b.Length + 1; j++)
                {
                    if (a[i - 1] == b[j - 1])
                    {
                        res[i, j] = res[i - 1, j - 1];
                    }
                    else
                    {
                        var modifications = new int[] { res[i - 1, j] + 1, res[i, j - 1] + 1, res[i - 1, j - 1] + 1 };
                        res[i, j] = modifications.Min();
                    }
                }
            }

            return res[a.Length, b.Length];
        }

        private class StaticOrInstanceFilter : SimpleSymbolFilter 
        {
            private readonly bool _onlyStatic;

            public StaticOrInstanceFilter(bool onlyStatic)
            {
                _onlyStatic = onlyStatic;
            }

            public override ResolveErrorType ErrorType
            {
                get
                {
                    return ResolveErrorType.NOT_RESOLVED;
                }
            }

            public override bool Accepts(IDeclaredElement declaredElement, ISubstitution substitution)
            {
                return _onlyStatic ? ((IModifiersOwner)declaredElement).IsStatic : !((IModifiersOwner)declaredElement).IsStatic;
            }
        }
    }
}
