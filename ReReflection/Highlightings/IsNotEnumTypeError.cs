using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Highlightings
{
    [StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    public class IsNotEnumTypeError : ReflectionHighlightingBase, IHighlighting
    {
        private readonly IInvocationExpression _invocation;

        public IsNotEnumTypeError([NotNull]IInvocationExpression invocation)
        {
            _invocation = invocation;
        }

        public override bool IsValid()
        {
            return _invocation.IsValid();
        }

        public override DocumentRange CalculateRange()
        {
            return _invocation.GetDocumentRange();
        }

        public string ToolTip
        {
            get
            {
                return string.Format("Type is not enum type.");
            }
        }

        public string ErrorStripeToolTip
        {
            get
            {
                return ToolTip;
            }
        }
    }
}
