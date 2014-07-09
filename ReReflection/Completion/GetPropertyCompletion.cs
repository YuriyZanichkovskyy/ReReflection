using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Resolve;

namespace ReSharper.Reflection.Completion
{
    public class GetPropertyCompletion : MethodSpecificCompletion
    {
        public GetPropertyCompletion()
            : base(CLRDeclaredElementType.PROPERTY)
        {
        }

        protected override void ProvideMemberSpecificArguments(DeclaredElementInstance<ITypeMember> member, IList<string> arguments, bool requiresBindingFlags)
        {
            var property = (IProperty)member.Element;
            string argumentTypes = string.Format("new [] {{ {0} }}",
                string.Join(", ", property.Parameters
                .Select(p => Typeof(p, member.Substitution)))); //
            if (requiresBindingFlags)
            {
                /*Use the following overload
                 public PropertyInfo GetProperty(
	string name,
	BindingFlags bindingAttr,
	Binder binder,
	Type returnType,
	Type[] types,
	ParameterModifier[] modifiers
)
)*/
                arguments.Add("null"); //Binder
                arguments.Add(Typeof(property.ReturnType, member.Substitution)); //Return Type
                arguments.Add(argumentTypes);
                arguments.Add("null"); //ParameterModifiers //TODO
            }
            else
            {
                arguments.Add(argumentTypes);
            }
        }

        private string Typeof(IParameter parameter, ISubstitution substitution)
        {
            return string.Format("typeof({0})", substitution.Apply(parameter.Type).GetPresentableName(CSharpLanguage.Instance));
        }

        private string Typeof(IType type, ISubstitution substitution)
        {
            return string.Format("typeof({0})", substitution.Apply(type).GetPresentableName(CSharpLanguage.Instance));
        }
    }
}
