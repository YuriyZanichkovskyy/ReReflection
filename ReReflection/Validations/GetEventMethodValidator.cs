using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Validations
{
    public class GetEventMethodValidator : ReflectionMemberValidatorBase
    {
        public GetEventMethodValidator(IMethod reflectionMethod, int? bindingFlagArgumentPosition = null) 
            : base(reflectionMethod, CLRDeclaredElementType.EVENT, bindingFlagArgumentPosition)
        {
        }

        protected override bool ProcessAmbigiousMembers(ITypeMember[] ambigious, out ITypeMember resolveMember)
        {
            resolveMember = null;
            return false;
        }
    }
}
