using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Completion
{
    public class ReflectionMemberLookupItem : CSharpDeclaredElementLookupItem
    {
        private readonly string _code;

        public ReflectionMemberLookupItem(string name, string code, DeclaredElementInstance instance, IElementPointerFactory elementPointerFactory, ILookupItemsOwner owner)
            : base(name, instance, elementPointerFactory, owner)
        {
            _code = code;
        }

        protected override string GetText()
        {
            return _code;
        }
    }
}
