using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReReflection.Highlightings;

namespace ReReflection.Validations
{
    public class GetFieldMethodValidator : ReflectionMemberValidatorBase
    {
        public GetFieldMethodValidator(IMethod reflectionMethod, int? bindingFlagArgumentPosition = null) 
            : base(reflectionMethod, CLRDeclaredElementType.FIELD, bindingFlagArgumentPosition)
        {
        }

        protected override bool ProcessAmbigiousMembers(ITypeMember[] ambigious, out ITypeMember resolveMember)
        {
            //It is not possible to have multiple fields with same name
            resolveMember = null;
            return false;
        }
    }
}
