using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Completion
{
    public class GetPropertyCompletion : MethodSpecificCompletion
    {
        public GetPropertyCompletion()
            : base(CLRDeclaredElementType.PROPERTY)
        {
        }

        protected override void ProvideMemberSpecificArguments(ITypeMember member, IList<string> arguments, bool requiresBindingFlags)
        {
            var property = (IProperty)member;
            string argumentTypes = string.Format("new [] {{ {0} }}",
                string.Join(", ", property.Parameters.Select(Typeof))); //
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
                arguments.Add(Typeof(property.ReturnType)); //Return Type
                arguments.Add(argumentTypes);
                arguments.Add("null"); //ParameterModifiers //TODO
            }
            else
            {
                arguments.Add(argumentTypes);
            }
        }

        private string Typeof(IParameter parameter)
        {
            return string.Format("typeof({0})", parameter.Type);
        }

        private string Typeof(IType type)
        {
            return string.Format("typeof({0})", type);
        }
    }
}
