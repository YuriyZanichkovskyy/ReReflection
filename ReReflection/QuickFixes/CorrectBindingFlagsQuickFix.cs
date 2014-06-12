using System;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.QuickFixes
{
    [QuickFix]
    public sealed class CorrectBindingFlagsQuickFix : QuickFixBase
    {
        private readonly IncorrectBindingFlagsError _error;

        public CorrectBindingFlagsQuickFix(IncorrectBindingFlagsError error)
        {
            _error = error;
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            var elementFactory = CSharpElementFactory.GetInstance(_error.Invocation);
            ICSharpArgumentsOwner arguments = _error.Invocation;
            var correctFlagsExpression = elementFactory.CreateExpression(_error.ExpectedBindingflags.GetFullString());
            if (_error.Invocation.Arguments.Count == 1)
            {
                var arg = arguments.AddArgumentBefore(_error.Invocation.Arguments[0], null);
                arg.SetValue(correctFlagsExpression);
            }
            else if (_error.Invocation.Arguments.Count > 1)
            {
                _error.Invocation.Arguments[1].SetValue(correctFlagsExpression);
            }
            return null;
        }

        public override string Text
        {
            get
            {
                if (_error.Method.Parameters.Count == 1) //BindingFlags arguments not specified at all
                {
                    return string.Format("Add correct BindingFlags argument.");
                }

                return string.Format("Fix BindingFlags argument.");
            }
        }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            return _error.IsValid() && _error.Invocation.IsValid();
        }
    }
}
