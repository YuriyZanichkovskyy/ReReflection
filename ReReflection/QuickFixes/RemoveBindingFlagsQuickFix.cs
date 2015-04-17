using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
#if R9
using JetBrains.ReSharper.Feature.Services.QuickFixes;
#else
using JetBrains.ReSharper.Intentions.Extensibility;
#endif
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.QuickFixes
{
    [QuickFix]
    public sealed class RemoveBindingFlagsQuickFix : QuickFixBase
    {
        private readonly BindingFlagsCanBeSkippedWarning _warning;

        public RemoveBindingFlagsQuickFix(BindingFlagsCanBeSkippedWarning warning)
        {
            _warning = warning;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var argumentsOwner = _warning.Invocation;
            argumentsOwner.RemoveArgument(_warning.Invocation.Arguments[1]);
            return null;
        }

        public override string Text
        {
            get
            {
                return "Remove BindingFlags argument (Default value will work).";
            }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return _warning.IsValid();
        }
    }
}
