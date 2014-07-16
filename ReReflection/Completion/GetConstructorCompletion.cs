using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;

namespace ReSharper.Reflection.Completion
{
    public class GetConstructorCompletion : MethodSpecificCompletion
    {
        public GetConstructorCompletion()
            : base(CLRDeclaredElementType.CONSTRUCTOR)
        {
        }

        protected override void ProvideMemberSpecificArguments(DeclaredElementInstance<ITypeMember> member, IList<string> arguments, bool requiresBindingFlags)
        {
            var method = (IMethod)member.Element;
            string argumentTypes = string.Format("new Type[] {{ {0} }}",
                string.Join(", ", method.Parameters
                .Select(p => Typeof(p, member.Substitution)))); //

            if (requiresBindingFlags)
            {
                arguments.Add("null"); //Binder
                arguments.Add(argumentTypes);
                arguments.Add("null"); //ParameterModifiers
            }
            else
            {
                arguments.Add(argumentTypes);
            }
        }

        protected override bool ShouldProvideMemberSpecificArguments(IList<ISymbolInfo> symbols)
        {
            return true;
        }

        protected override bool IncludeNameArgument
        {
            get { return false; }
        }
    }
}
