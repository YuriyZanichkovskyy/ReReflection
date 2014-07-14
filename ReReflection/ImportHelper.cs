using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReSharper.Reflection
{
    public static class ImportHelper
    {
        public static void AddMissingNamespaceImport(IReference reference, CSharpElementFactory factory, string importName)
        {
            var importScope = CSharpReferenceBindingUtil.GetImportScope(reference);
            AddMissingNamespaceImport(importScope, factory, importName);
        }

        public static void AddMissingNamespaceImport(ICSharpTypeAndNamespaceHolderDeclaration importScope, CSharpElementFactory factory, string importName)
        {
            var importedNamespace = GetNamespace(factory, importName);
            if (!UsingUtil.CheckAlreadyImported(importScope, importedNamespace))
            {
                UsingUtil.AddImportTo(importScope, importedNamespace);
            }
        }

        private static INamespace GetNamespace(CSharpElementFactory factory, string importName)
        {
            var usingDirective = factory.CreateUsingDirective(importName);
            var reference = usingDirective.ImportedSymbolName;
            var reflectionNamespace = reference.Reference.Resolve().DeclaredElement as INamespace;
            return reflectionNamespace;
        }
    }
}
