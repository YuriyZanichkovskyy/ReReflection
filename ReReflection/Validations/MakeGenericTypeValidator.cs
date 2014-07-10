using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Resources;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.Validations
{
    public class MakeGenericTypeValidator : ReflectionTypeMethodValidatorBase
    {
        public MakeGenericTypeValidator(IMethod reflectionMethod) : base(reflectionMethod)
        {
        }

        public override bool CanValidate(ReflectedTypeResolveResult reflectedType)
        {
            return reflectedType.ResolvedAs == ReflectedTypeResolution.Exact || reflectedType.ResolvedAs == ReflectedTypeResolution.ExactMakeGeneric;
        }

        public override ReflectionHighlightingBase Validate(ReflectedTypeResolveResult resolvedType, IInvocationExpression invocation)
        {
            var reference = invocation.InvokedExpression as IReferenceExpression;

            if (resolvedType.TypeElement == null)
                return null;

            if (resolvedType.TypeElement.TypeParameters.Count == 0)
            {
                return new IncorrectMakeGenericTypeHighlighting(reference,
                    string.Format("Type '{0}' is not a generic type.", 
                    resolvedType.Type.GetPresentableName(CSharpLanguage.Instance)));
            }

            if (resolvedType.Type.IsResolved)
            {
                return new IncorrectMakeGenericTypeHighlighting(reference,
                    string.Format("Type '{0}' is closed generic type.", 
                    resolvedType.Type.GetPresentableName(CSharpLanguage.Instance)));
            }

            int typeArgumentCount = invocation.Arguments.Count;
            if (typeArgumentCount != 0)
            {
                var typeParameters = invocation.Arguments[0].Expression as IArrayCreationExpression;

                if (typeParameters != null && typeParameters.ArrayInitializer != null)
                {
                    if (typeParameters.ArrayInitializer.ElementInitializers.Count != resolvedType.TypeElement.TypeParameters.Count)
                    {
                        return new IncorrectMakeGenericTypeHighlighting(invocation.Arguments[0],
                            string.Format("Incorrect count of type parameters for type {0}.",
                            resolvedType.Type.GetPresentableName(CSharpLanguage.Instance)));
                    }
                }
            }
            
            if (typeArgumentCount != resolvedType.TypeElement.TypeParameters.Count)
            {
                var offset = invocation.LPar.GetTreeStartOffset();
                var treeTextRange = new TreeTextRange(offset, invocation.RPar.GetTreeEndOffset());
                return new IncorrectMakeGenericTypeHighlighting(invocation,
                        string.Format("Incorrect count of type parameters for type {0}.",
                        resolvedType.Type.GetPresentableName(CSharpLanguage.Instance)),
                        IsValidTreeTextRange(treeTextRange) ? invocation.GetContainingFile().GetDocumentRange(treeTextRange) : (DocumentRange?) null);
            }

            return null;
        }

        private bool IsValidTreeTextRange(TreeTextRange range)
        {
            return range.StartOffset != TreeOffset.InvalidOffset && range.EndOffset != TreeOffset.InvalidOffset;
        }
    }
}
