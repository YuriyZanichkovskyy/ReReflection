using System.Reflection;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection.Highlightings
{
    [StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    public class ReflectionMemberNotFoundError : ReflectionHighlightingBase, IHighlighting
    {
        public BindingFlags? BindingFlags { get; private set; }
        private readonly IExpression _nameArgument;
        private readonly DeclaredElementType _elementType;
        private readonly ITypeElement _type;

        public ReflectionMemberNotFoundError(IExpression nameArgument, DeclaredElementType elementType, ITypeElement type, BindingFlags? bindingFlags)
        {
            BindingFlags = bindingFlags;
            _nameArgument = nameArgument;
            _elementType = elementType;
            _type = type;
        }

        public override bool IsValid()
        {
            return NameArgument.IsValid();
        }

        public string ToolTip
        {
            get
            {
                string memberType = ElementType == null ? "member" : ElementType.ToString();
                return string.Format("Type {0} does not contain {1} with name '{2}'", Type, memberType, NameArgument.ConstantValue.Value);
            }
        }

        public string ErrorStripeToolTip
        {
            get
            {
                return ToolTip;
            }
        }

        public IExpression NameArgument
        {
            get { return _nameArgument; }
        }

        public string NameArgumentValue
        {
            get { return (string) NameArgument.ConstantValue.Value; }
        }

        public DeclaredElementType ElementType
        {
            get { return _elementType; }
        }

        public ITypeElement Type
        {
            get { return _type; }
        }

        public override DocumentRange CalculateRange()
        {
            return NameArgument.GetDocumentRange();
        }
    }
}
