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
            return ReflectedTypeHelper.ResolveReflectedType(invocationExpression);
        }
    }
}
