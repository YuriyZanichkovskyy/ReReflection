using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.Util;

namespace ReSharper.Reflection.Search
{
    [PsiSharedComponent]
    public class ReflectedMemberSearcherFactory : IDomainSpecificSearcherFactory
    {
        private readonly SearchDomainFactory _searchDomainFactory;

        public ReflectedMemberSearcherFactory(SearchDomainFactory searchDomainFactory)
        {
            _searchDomainFactory = searchDomainFactory;
        }

        public bool IsCompatibleWithLanguage(PsiLanguageType languageType)
        {
            return languageType.Equals(CSharpLanguage.Instance);
        }

        public IDomainSpecificSearcher CreateConstructorSpecialReferenceSearcher(ICollection<IConstructor> constructors)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateMethodsReferencedByDelegateSearcher(IDelegate @delegate)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateReferenceSearcher(ICollection<IDeclaredElement> elements, bool findCandidates)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateTextOccurenceSearcher(ICollection<IDeclaredElement> elements)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateTextOccurenceSearcher(string subject)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateLateBoundReferenceSearcher(ICollection<IDeclaredElement> elements)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateAnonymousTypeSearcher(IList<AnonymousTypeDescriptor> typeDescription, bool caseSensitive)
        {
            return null;
        }

        public IDomainSpecificSearcher CreateConstantExpressionSearcher(ConstantValue constantValue, bool onlyLiteralExpression)
        {
            return null;
        }

        public IEnumerable<string> GetAllPossibleWordsInFile(IDeclaredElement element)
        {
            return new string[0];
        }

        public IEnumerable<Pair<IDeclaredElement, Predicate<FindResult>>> GetRelatedDeclaredElements(IDeclaredElement element)
        {
            return new Pair<IDeclaredElement, Predicate<FindResult>>[0];
        }

        public JetTuple<ICollection<IDeclaredElement>, Predicate<IFindResultReference>, bool> GetDerivedFindRequest(IFindResultReference result)
        {
            return null;
        }

        public JetTuple<ICollection<IDeclaredElement>, bool> GetNavigateToTargets(IDeclaredElement element)
        {
            return null;
        }

        public ICollection<FindResult> TransformNavigationTargets(ICollection<FindResult> targets)
        {
            return targets;
        }

        public ISearchDomain GetDeclaredElementSearchDomain(IDeclaredElement declaredElement)
        {
            return _searchDomainFactory.CreateSearchDomain(declaredElement.GetPsiServices().Solution, false);
        }
    }
}
