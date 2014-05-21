using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Features.Browsing.Resources;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;

namespace ReReflection.Completion
{
    public class MethodSpecificCompletion
    {
        public MethodSpecificCompletion(DeclaredElementType expectedMemberType)
        {
            ExpectedMemberType = expectedMemberType;
        }

        protected DeclaredElementType ExpectedMemberType { get; private set; }

        public void ProcessMembers(CSharpCodeCompletionContext context, GroupedItemsCollector collector, IEnumerable<ITypeMember> members)
        {
            ITypeMember[] filteredMembers;
            if (ExpectedMemberType != null)
            {
                filteredMembers = members.Where(m => m.GetElementType() == ExpectedMemberType).ToArray();
            }
            else
            {
                filteredMembers = members.ToArray();
            }

            var membersByName = filteredMembers.ToLookup(m => m.ShortName);

            foreach (var member in filteredMembers)
            {
                string nameArgument = string.Format("\"{0}\"", member.ShortName);

                IList<string> arguments = new List<string>();
                arguments.Add(nameArgument); //always present

                if (NeedsBindingFlags(member))
                {
                    arguments.Add(GetExpectedBindingFlags(member).GetFullString());
                }

                if (membersByName[member.ShortName].HasMultiple()) //additional arguments needs to be provided
                {
                    ProvideMemberSpecificArguments(member, arguments, NeedsBindingFlags(member));
                }

                var lookupItem = new ReflectionMemberLookupItem(member.ShortName,
                    string.Join(", ", arguments.ToArray()),
                    new DeclaredElementInstance<ITypeMember>(member),
                    context,
                    context.BasicContext.LookupItemsOwner);

                lookupItem.InitializeRanges(context.CompletionRanges, context.BasicContext);
                lookupItem.OrderingString = string.Format("__A_MEMBER_{0}", member.ShortName); //
                collector.AddAtDefaultPlace(lookupItem);
            }
        }

        protected virtual void ProvideMemberSpecificArguments(ITypeMember member, IList<string> arguments, bool requiresBindingFlags)
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
