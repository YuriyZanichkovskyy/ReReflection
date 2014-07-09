using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Highlightings
{
    [StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    public class IncorrectMakeArrayTypeHighlighting : ReflectionHighlightingBase, IHighlighting
    {
        private readonly ITreeNode _node;
        private readonly string _message;

        public IncorrectMakeArrayTypeHighlighting(ITreeNode node, string message)
        {
            _node = node;
            _message = message;
        }

        public string ToolTip
        {
            get
            {
                return _message;
            }
        }

        public string ErrorStripeToolTip
        {
            get
            {
                return ToolTip;
            }
        }

        public override bool IsValid()
        {
            return _node.IsValid();
        }

        public override DocumentRange CalculateRange()
        {
            return _node.GetDocumentRange();
        }
    }
}
