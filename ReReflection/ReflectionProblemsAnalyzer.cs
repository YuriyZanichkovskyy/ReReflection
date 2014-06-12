using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReSharper.Reflection.Highlightings;
using ReSharper.Reflection.Services;

namespace ReSharper.Reflection
{
    [ElementProblemAnalyzer(new[] { typeof(IInvocationExpression) }, 
        HighlightingTypes = new[] { typeof(ReflectionMemberNotFoundError), typeof(IncorrectMakeGenericTypeHighlighting) })]
    public class ReflectionProblemsAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
    {
        protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            IMethod method;
            if (ReflectedTypeHelper.IsReflectionTypeMethod(element, out method))
            {
                var reflectedType = ResolveReflectedType(element);
                var validator = ReflectionValidatorsRegistry.GetValidator(method);
                if (validator != null && validator.CanValidate(reflectedType))
                {
                    var error = validator.Validate(reflectedType, element);
                    if (error != null)
                    {
                        consumer.AddHighlighting(error);
                    }
                }
            }
        }

        private ReflectedTypeResolveResult ResolveReflectedType(IInvocationExpression invocationExpression)
        {
            var referenceExpression = invocationExpression.InvokedExpression as IReferenceExpression;

            if (referenceExpression != null)
            {
                var typeOfExpression = referenceExpression.QualifierExpression as ITypeofExpression;
                if (typeOfExpression != null)
                {
                    var type = typeOfExpression.ArgumentType.GetTypeElement<ITypeElement>();
                    if (type == null)
                    {
                        return ReflectedTypeResolveResult.NotResolved;
                    }

                    return new ReflectedTypeResolveResult(type, ReflectedTypeResolution.Exact); 
                }
                var methodInvocationExpression = referenceExpression.QualifierExpression as IInvocationExpression;
                if (methodInvocationExpression != null && ReflectedTypeHelper.IsReflectionTypeMethod(invocationExpression, "MakeGenericType"))
                {
                    var resolvedType = ResolveReflectedType(methodInvocationExpression);
                    if (resolvedType.ResolvedAs == ReflectedTypeResolution.Exact)
                    {
                        return new ReflectedTypeResolveResult(resolvedType.TypeElement, ReflectedTypeResolution.ExactMakeGeneric);
                    }
                }
            }

            return ReflectedTypeResolveResult.NotResolved;
        }
    }
}
