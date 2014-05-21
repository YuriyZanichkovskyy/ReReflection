using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReReflection.Completion
{
    public class GetMethodCompletion : MethodSpecificCompletion
    {
        public GetMethodCompletion()
            : base(CLRDeclaredElementType.METHOD)
        {
        }

        protected override void ProvideMemberSpecificArguments(ITypeMember member, IList<string> arguments, bool requiresBindingFlags)
        {
            //means that we have overloads
            var method = (IMethod)member;
            //TODO : doesn't work with out and ref arguments..., what to do with generic methods...
            string argumentTypes = string.Format("new [] {{ {0} }}",
                string.Join(", ", method.Parameters.Select(Typeof))); //
            if (requiresBindingFlags)
            {
                /*Use the following overload
                 * public MethodInfo GetMethod(
	string name,
	BindingFlags bindingAttr,
	Binder binder,
	Type[] types,
	ParameterModifier[] modifiers
)*/
                arguments.Add("null"); //Binder
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
    }
}
