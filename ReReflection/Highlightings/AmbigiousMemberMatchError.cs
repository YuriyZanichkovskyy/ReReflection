using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
#if R9
using JetBrains.ReSharper.Feature.Services.Daemon;
#else

#endif

using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

[assembly: RegisterConfigurableSeverity("ReflectionAmbigiousMatch", null, "CodeInfo", "Some title", "other title", Severity.WARNING,  false)]

namespace ReSharper.Reflection.Highlightings
{
    //[StaticSeverityHighlighting(Severity.ERROR, "Reflection", OverlapResolve = OverlapResolveKind.ERROR)]
    [ConfigurableSeverityHighlighting("ReflectionAmbigiousMatch", "CSHARP")]
    public class AmbigiousMemberMatchError : ReflectionHighlightingBase, IHighlighting
    {
        private readonly IExpression _nameArgument;
        private readonly DeclaredElementType _elementType;
        private readonly string _suggestionMessage;

        public AmbigiousMemberMatchError(IExpression nameArgument, DeclaredElementType elementType, string suggestionMessage)
        {
            _nameArgument = nameArgument;
            _elementType = elementType;
            _suggestionMessage = suggestionMessage;
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
                return string.Format("Ambigious {0} with name '{1}'. {2}", 
                    memberType, _nameArgument.ConstantValue.Value, _suggestionMessage);
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
