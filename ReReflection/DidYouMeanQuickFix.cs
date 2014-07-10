using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve.Filters;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection
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


    [QuickFix]
    public class DidYouMeanQuickFix : DidYouMeanQuickFixBase
    {
        private readonly NotResolvedError _error;
        private IAccessContext _accessContext;

        public DidYouMeanQuickFix(NotResolvedError error) 
            : base((ICSharpExpression) error.Reference.GetTreeNode(), error.Reference.GetName())
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

    [QuickFix]
    public class DidYouMeanQuickFix2 : DidYouMeanQuickFixBase
    {
        private readonly ReflectionMemberNotFoundError _error;

        public DidYouMeanQuickFix2(ReflectionMemberNotFoundError error) 
            : base((ICSharpExpression) error.NameArgument, error.NameArgumentValue)
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
