using System;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReReflection.Highlightings;

namespace ReReflection.Validations
{
    public class MakeGenericTypeValidator : ReflectionTypeMethodValidatorBase
    {
        public MakeGenericTypeValidator(IMethod reflectionMethod) : base(reflectionMethod)
        {
        }

        public override bool CanValidate(ReflectedTypeResolveResult reflectedType)
        {
            return reflectedType.ResolvedAs == ReflectedTypeResolution.Exact;
        }

        public override ReflectionHighlightingBase Validate(ReflectedTypeResolveResult resolvedType, IInvocationExpression invocation)
        {
            var reference = invocation.InvokedExpression as IReferenceExpression;

            if (resolvedType.TypeElement.TypeParameters.Count == 0)
            {
                return new IncorrectMakeGenericTypeHighlighting(reference,
                    string.Format("Type '{0}' is not a generic type.", resolvedType.TypeElement.GetClrName()));
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
                            string.Format("Incorrect count of type parameters for type {0}.", resolvedType.TypeElement.GetClrName()));
                    }
                }
            }
            
            if (typeArgumentCount != resolvedType.TypeElement.TypeParameters.Count)
            {
                var offset = invocation.LPar.GetTreeStartOffset();
                var treeTextRange = new TreeTextRange(offset, invocation.RPar.GetTreeEndOffset());
                return new IncorrectMakeGenericTypeHighlighting(invocation,
                        string.Format("Incorrect count of type parameters for type {0}.", resolvedType.TypeElement.GetClrName()),
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
