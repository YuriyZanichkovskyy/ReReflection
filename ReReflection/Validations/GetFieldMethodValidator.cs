using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Validations
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
