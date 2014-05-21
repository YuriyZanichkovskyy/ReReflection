using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReReflection.Completion
{
    public class GetMemberCompletion : MethodSpecificCompletion
    {
        private static readonly IDictionary<DeclaredElementType, MemberTypes> ElementTypeToMemberType =
            new Dictionary<DeclaredElementType, MemberTypes>()
            {
                { CLRDeclaredElementType.PROPERTY, MemberTypes.Property },
                { CLRDeclaredElementType.EVENT, MemberTypes.Event },
                { CLRDeclaredElementType.FIELD, MemberTypes.Field },
                { CLRDeclaredElementType.CONSTRUCTOR, MemberTypes.Constructor },
                { CLRDeclaredElementType.METHOD, MemberTypes.Method },
                //etc..
            };

        public GetMemberCompletion()
            : base(null)
        {
        }

        protected override void ProvideMemberSpecificArguments(ITypeMember member, IList<string> arguments, bool requiresBindingFlags)
        {
            var elementType = member.GetElementType();
            arguments.Insert(1, ElementTypeToMemberType[elementType].GetFullString());
        }
    }
}
