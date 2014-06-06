using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.ReSharper.Psi.Resources;
using JetBrains.TextControl;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace ReReflection.Completion
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
