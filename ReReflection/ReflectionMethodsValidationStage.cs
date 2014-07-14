using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Daemon.Stages;
using JetBrains.ReSharper.Daemon.UsageChecking;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReSharper.Reflection.Services;

namespace ReSharper.Reflection
{
    [DaemonStage(StagesAfter = new Type[] { typeof(CollectUsagesStage), typeof(CSharpErrorStage) },
        StagesBefore = new Type[] { typeof(SmartResolverStage), typeof(GlobalFileStructureCollectorStage), typeof(IdentifierHighlightingStage) })]
    public class ReflectionMethodsValidationStage : CSharpDaemonStageBase
    {
        protected override IDaemonStageProcess CreateProcess(IDaemonProcess process, IContextBoundSettingsStore settings,
            DaemonProcessKind processKind, ICSharpFile file)
        {
            return new ReflectionMethodsValidationProcess(process, settings, file);
        }
    }

    public class ReflectionMethodsValidationProcess : CSharpIncrementalDaemonStageProcessBase
    {
        public ReflectionMethodsValidationProcess(IDaemonProcess process, IContextBoundSettingsStore settingsStore, ICSharpFile file) 
            : base(process, settingsStore, file)
        {
        }

        public override void VisitInvocationExpression(IInvocationExpression element, IHighlightingConsumer consumer)
        {
            base.VisitInvocationExpression(element, consumer);
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
