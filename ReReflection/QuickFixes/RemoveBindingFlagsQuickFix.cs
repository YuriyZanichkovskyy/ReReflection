using System;
using System.Collections.Generic;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Intentions.Extensibility.Menu;
using JetBrains.TextControl;
using JetBrains.Util;
using ReReflection.Highlightings;

namespace ReReflection.QuickFixes
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
