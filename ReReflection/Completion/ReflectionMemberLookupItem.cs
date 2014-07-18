using System.Collections.Generic;
using System.Linq;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExpectedTypes;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Transactions;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReSharper.Reflection.Completion
{
    public class ReflectionMemberLookupItem : CSharpDeclaredElementLookupItem
    {
        private readonly string _code;
        private readonly CSharpCodeCompletionContext _context;

        public ReflectionMemberLookupItem(string name, string code, DeclaredElementInstance instance, CSharpCodeCompletionContext context, ILookupItemsOwner owner)
            : base(name, instance, context, owner)
        {
            _code = code;
            _context = context;
        }

        protected CSharpCodeCompletionContext Context
        {
            get { return _context; }
        }

        protected override string GetText()
        {
            return _code;
        }

        protected override void OnAfterComplete(ITextControl textControl, ref TextRange nameRange, ref TextRange decorationRange, TailType tailType,
            ref Suffix suffix, ref IRangeMarker caretPositionRangeMarker)
        {
            Solution.GetPsiServices().Files.CommitAllDocuments();
            var psiServices = Solution.GetPsiServices();
            using (var transactionCookie = new PsiTransactionCookie(psiServices, DefaultAction.Rollback, "Lookup item"))
            {
                ImportHelper.AddMissingNamespaceImport(
                    (ICSharpTypeAndNamespaceHolderDeclaration) Context.BasicContext.File,
                    CSharpElementFactory.GetInstance(Context.PsiModule), "System.Reflection");
                psiServices.Caches.Update();
                transactionCookie.Commit();
            }
        }
    }
}
