using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.Util;

namespace ReSharper.Reflection.Completion
{
    public class MethodSpecificCompletion
    {
        public MethodSpecificCompletion(DeclaredElementType expectedMemberType)
        {
            ExpectedMemberType = expectedMemberType;
        }

        protected DeclaredElementType ExpectedMemberType { get; private set; }

        public void ProcessMembers(CSharpCodeCompletionContext context, GroupedItemsCollector collector, ISymbolTable symbols)
        {
            symbols.ForAllSymbolInfos(symbol =>
            {
                var member = symbol.GetDeclaredElement() as ITypeMember;
                if (member != null && member.GetElementType() == ExpectedMemberType)
                {
                    string nameArgument = string.Format("\"{0}\"", symbol.ShortName);
                    var declaredElementInstance = new DeclaredElementInstance<ITypeMember>((ITypeMember) symbol.GetDeclaredElement(), 
                        symbol.GetSubstitution());

                    if (!IncludeSymbol(declaredElementInstance))
                        return;
                    

                    IList<string> arguments = new List<string>();
                    arguments.Add(nameArgument); //always present

                    if (NeedsBindingFlags(member))
                    {
                        arguments.Add(GetExpectedBindingFlags(member).GetFullString());
                    }

                    if (symbols.GetSymbolInfos(member.ShortName).HasMultiple()) //additional arguments needs to be provided
                    {
                        ProvideMemberSpecificArguments(declaredElementInstance, arguments, NeedsBindingFlags(member));
                    }

                    var lookupItem = new ReflectionMemberLookupItem(symbol.ShortName,
                        string.Join(", ", arguments.ToArray()),
                        declaredElementInstance,
                        context,
                        context.BasicContext.LookupItemsOwner);

                    lookupItem.InitializeRanges(context.CompletionRanges, context.BasicContext);
                    lookupItem.OrderingString = string.Format("__A_MEMBER_{0}", symbol.ShortName); //
                    collector.AddToTop(lookupItem);
                }
            });
        }

        protected virtual bool IncludeSymbol(DeclaredElementInstance<ITypeMember> member)
        {
            return true;
        }

        protected virtual void ProvideMemberSpecificArguments(DeclaredElementInstance<ITypeMember> member, IList<string> arguments, bool requiresBindingFlags)
        {
        }

        protected virtual bool NeedsBindingFlags(ITypeMember member)
        {
            //By default Public | Static | Instance are included
            return member.GetAccessRights() != AccessRights.PUBLIC;
        }

        protected virtual BindingFlags GetExpectedBindingFlags(ITypeMember member)
        {
            BindingFlags flags = 0;
            if (member.IsStatic)
            {
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }
            if (member.GetAccessRights() != AccessRights.PUBLIC)
            {
                flags |= BindingFlags.NonPublic;
            }
            else
            {
                flags |= BindingFlags.Public;
            }

            return flags;
        }
    }
}
