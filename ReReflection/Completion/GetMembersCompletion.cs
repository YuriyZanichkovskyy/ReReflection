using System.Collections.Generic;
using System.Reflection;
using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Completion
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

        protected override void ProvideMemberSpecificArguments(DeclaredElementInstance<ITypeMember> member, IList<string> arguments, bool requiresBindingFlags)
        {
            var elementType = member.Element.GetElementType();
            arguments.Insert(1, ElementTypeToMemberType[elementType].GetFullString());
        }
    }
}
