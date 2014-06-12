using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Highlightings
{
    [StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    public class ReflectionMemberNotFoundError : ReflectionHighlightingBase, IHighlighting
    {
        private readonly IExpression _nameArgument;
        private readonly DeclaredElementType _elementType;
        private readonly ITypeElement _type;

        public ReflectionMemberNotFoundError(IExpression nameArgument, DeclaredElementType elementType, ITypeElement type)
        {
            _nameArgument = nameArgument;
            _elementType = elementType;
            _type = type;
        }

        public override bool IsValid()
        {
            return _nameArgument.IsValid();
        }

        public string ToolTip
        {
            get
            {
                string memberType = _elementType == null ? "member" : _elementType.ToString();
                return string.Format("Type {0} does not contain {1} with name '{2}'", _type, memberType, _nameArgument.ConstantValue.Value);
            }
        }

        public string ErrorStripeToolTip
        {
            get
            {
                return ToolTip;
            }
        }

        public override DocumentRange CalculateRange()
        {
            return _nameArgument.GetDocumentRange();
        }
    }
}
