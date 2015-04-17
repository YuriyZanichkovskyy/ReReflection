using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
#if R9
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
#else
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
#endif
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharper.Reflection.QuickFixes
{
    public class ReplaceWithSimilarName : BulbActionBase
    {
        private readonly ICSharpExpression _referenceExpression;
        private readonly IDeclaredElement _element;
        private readonly PsiLanguageType _language;
        private readonly Func<ICSharpExpression, IDeclaredElement, ICSharpExpression> _createReplacementExpression;

        public ReplaceWithSimilarName([NotNull] ICSharpExpression referenceExpression,
            [NotNull] IDeclaredElement element,
            PsiLanguageType language,
            [NotNull] Func<ICSharpExpression, IDeclaredElement, ICSharpExpression> createReplacementExpression)
        {
            if (referenceExpression == null) throw new ArgumentNullException("referenceExpression");
            if (element == null) throw new ArgumentNullException("element");
            if (createReplacementExpression == null) throw new ArgumentNullException("createReplacementExpression");

            _referenceExpression = referenceExpression;
            _element = element;
            _language = language;
            _createReplacementExpression = createReplacementExpression;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            _referenceExpression.ReplaceBy(_createReplacementExpression(_referenceExpression, _element));
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

    public abstract class DidYouMeanQuickFixBase : IQuickFix
    {
        private readonly ICSharpExpression _expression;
        private readonly string _referenceName;
        private const int _threshold = 2;

        protected DidYouMeanQuickFixBase(ICSharpExpression expression, string referenceName)
        {
            _expression = expression;
            _referenceName = referenceName;
        }

        public IEnumerable<IntentionAction> CreateBulbItems()
        {
            return GetMostSimilarNamesBulbActions().ToQuickFixAction();
        }

        protected virtual void ConfigureFilters(IList<ISymbolFilter> filters)
        {

        }

        private IEnumerable<ReplaceWithSimilarName> GetMostSimilarNamesBulbActions()
        {
            bool? isStaticReference;
            ITypeElement typeElement = GetTypeElement(out isStaticReference);
            var language = _expression.Language;

            if (typeElement != null)
            {
                var symbolFilters = new List<ISymbolFilter>();
                if (isStaticReference.HasValue)
                {
                    symbolFilters.Add(new StaticOrInstanceFilter(isStaticReference.Value));
                }

                ConfigureFilters(symbolFilters);

                var symbolTable = ResolveUtil.GetSymbolTableByTypeElement(typeElement, SymbolTableMode.FULL, _expression.GetPsiModule());
                symbolTable = symbolTable.Filter(symbolFilters.ToArray());

                IList<ReplaceWithSimilarName> result = new List<ReplaceWithSimilarName>();
                HashSet<string> names = new HashSet<string>();
                symbolTable.ForAllSymbolInfos(s =>
                {
                    var symbolDeclaredElement = s.GetDeclaredElement();
                    if (!names.Contains(symbolDeclaredElement.ShortName))
                    {
                        names.Add(symbolDeclaredElement.ShortName);
                        if (CalculateLevenshteinDistance(_referenceName, symbolDeclaredElement.ShortName) <= _threshold)
                        {
                            result.Add(new ReplaceWithSimilarName(_expression, s.GetDeclaredElement(), language, CreateReplacementExpression));
                        }
                    }
                });

                return result;
            }

            return Enumerable.Empty<ReplaceWithSimilarName>();
        }

        protected abstract ITypeElement GetTypeElement(out bool? isStaticReference);

        protected abstract ICSharpExpression CreateReplacementExpression(ICSharpExpression expression, IDeclaredElement declaredElement);

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
