using System.Reflection;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resx.Utils;
using JetBrains.ReSharper.Psi.Tree;

namespace ReReflection.Highlightings
{
    [StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    public class IncorrectBindingFlagsError : ReflectionHighlightingBase, IHighlighting
    {
        private readonly IMethod _method;
        private readonly IInvocationExpression _invocation;
        private readonly IExpression _bindingFlagsExpression;
        private readonly BindingFlags _expectedBindingflags;
        private readonly ITypeMember _member;

        public IncorrectBindingFlagsError(IMethod method, IInvocationExpression invocation, IExpression bindingFlagsExpression, BindingFlags expectedBindingflags, ITypeMember member)
        {
            _method = method;
            _invocation = invocation;
            _bindingFlagsExpression = bindingFlagsExpression;
            _expectedBindingflags = expectedBindingflags;
            _member = member;
        }

        public string ToolTip
        {
            get
            {
                if (_bindingFlagsExpression == null)
                {
                    return string.Format("'{0}' should be specified for {1} '{2}' as BindingFlags argument.",
                        ExpectedBindingflags.GetFullString(),
                        _member.GetElementType(),
                        _member.ShortName);
                }

                return string.Format("Incorrect BindingFlags specified for {0} '{1}'. Should be '{2}'",
                        _member.GetElementType(),
                        _member.ShortName,
                        ExpectedBindingflags.GetFullString());
            }
        }

        public string ErrorStripeToolTip
        {
            get
            {
                return ToolTip;
            }
        }

        public IMethod Method
        {
            get
            {
                return _method;
            }
        }

        public IInvocationExpression Invocation
        {
            get
            {
                return _invocation;
            }
        }

        public BindingFlags ExpectedBindingflags
        {
            get
            {
                return _expectedBindingflags;
            }
        }

        public override bool IsValid()
        {
            return Invocation.IsValid();
        }

        public override DocumentRange CalculateRange()
        {
            if (_bindingFlagsExpression != null)
            {
                return _bindingFlagsExpression.GetDocumentRange();
            }

            return Invocation.GetDocumentRange();
        }
    }
}
