using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Resolve;

namespace ReSharper.Reflection.Completion
{
    public class GetMethodCompletion : MethodSpecificCompletion
    {
        public GetMethodCompletion()
            : base(CLRDeclaredElementType.METHOD)
        {
        }

        protected override void ProvideMemberSpecificArguments(DeclaredElementInstance<ITypeMember> member, IList<string> arguments, bool requiresBindingFlags)
        {
            //means that we have overloads
            var method = (IMethod)member.Element;
            //TODO : doesn't work with out and ref arguments..., what to do with generic methods...
            string argumentTypes = string.Format("new Type[] {{ {0} }}",
                string.Join(", ", method.Parameters
                .Select(p => Typeof(p, member.Substitution)))); //
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

        //protected override bool IncludeSymbol(DeclaredElementInstance<ITypeMember> member)
        //{
        //    var method = (IMethod)member.Element;
        //    return method.TypeParameters.Count == 0;
        //}

        
    }
}
