using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReReflection.Validations
{
    public class GetMethodMethodValidator : ReflectionMemberValidatorBase
    {
        public GetMethodMethodValidator(IMethod reflectionMethod, int? bindingFlagArgumentPosition = null) 
            : base(reflectionMethod, CLRDeclaredElementType.METHOD, bindingFlagArgumentPosition)
        {
        }

        protected override bool ProcessAmbigiousMembers(ITypeMember[] ambigious, out ITypeMember resolveMember)
        {
            throw new NotImplementedException();
        }
    }
}
