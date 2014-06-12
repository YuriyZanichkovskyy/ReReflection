using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Highlightings
{
    [StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    public class IncorrectMakeGenericTypeHighlighting : ReflectionHighlightingBase, IHighlighting
    {
        private readonly ITreeNode _node;
        private readonly string _message;
        private readonly DocumentRange? _range;

        public IncorrectMakeGenericTypeHighlighting(ITreeNode node, string message, DocumentRange? range = null)
        {
            _node = node;
            _message = message;
            _range = range;
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
            return _range ?? _node.GetDocumentRange();
        }
    }
}
