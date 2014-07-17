using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Reflection.Services;

namespace ReSharper.Reflection.Completion
{
    [Language(typeof(CSharpLanguage))]
    public class ReflectionMethodsCompletionProvider : ItemsProviderOfSpecificContext<CSharpCodeCompletionContext>
    {
        private readonly IDictionary<string, MethodSpecificCompletion> _completionForMethods = 
            new Dictionary<string, MethodSpecificCompletion>()
            {
                { "GetField", new MethodSpecificCompletion(CLRDeclaredElementType.FIELD) },
                { "GetMethod", new GetMethodCompletion() },
                { "GetEvent", new MethodSpecificCompletion(CLRDeclaredElementType.EVENT) },
                { "GetProperty", new GetPropertyCompletion() },
                { "GetMember", new GetMemberCompletion()},
                //{ "GetConstructor", new GetConstructorCompletion()}
            };


        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            CodeCompletionType codeCompletionType = context.BasicContext.CodeCompletionType;
            if (codeCompletionType != CodeCompletionType.SmartCompletion)
                return codeCompletionType != CodeCompletionType.ImportCompletion;
            else
                return false;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {           
            var node = context.NodeInFile;
            if (node.GetTokenType() == CSharpTokenType.LPARENTH && node.Parent is IInvocationExpression)
            {
                var invocationExpression = (IInvocationExpression) node.Parent;
                IMethod method;
                if (ReflectedTypeHelper.IsReflectionTypeMethod(invocationExpression, false, out method))
                {
                    MethodSpecificCompletion methodSpecificCompletion;
                    if (IsCompletionRegisteredForMethod(method, out methodSpecificCompletion))
                    {
                        var reflectedType = ReflectedTypeHelper.ResolveReflectedType(invocationExpression);
                        if (reflectedType.ResolvedAs != ReflectedTypeResolution.NotResolved)
                        {
                            methodSpecificCompletion.ProcessMembers(context, collector, 
                                reflectedType.Type.GetSymbolTable(context.PsiModule).Filter(new ExtensionMethodsFilter()));
                            collector.AddFilter(new ReflectionMembersPreference());
                        }
                    }
                }
            }

            return base.AddLookupItems(context, collector);
        }

        private bool IsCompletionRegisteredForMethod(IMethod method, out MethodSpecificCompletion methodSpecificCompletion)
        {
            return _completionForMethods.TryGetValue(method.ShortName, out methodSpecificCompletion);
        }

        private class ReflectionMembersPreference : ILookupItemsPreference
        {
            public IEnumerable<ILookupItem> FilterItems(ICollection<ILookupItem> items)
            {
                return items.OfType<ReflectionMemberLookupItem>();
            }

            public int Order
            {
                get
                {
                    return 100;
                }
            }
        }

        private class ExtensionMethodsFilter : SimpleSymbolInfoFilter
        {
            public override bool Accepts(ISymbolInfo datum)
            {
                var method = datum.GetDeclaredElement() as IMethod;
                return method == null || !method.IsExtensionMethod;
            }

            public override ResolveErrorType ErrorType
            {
                get { return ResolveErrorType.NOT_RESOLVED; }
            }
        }
    }
}
