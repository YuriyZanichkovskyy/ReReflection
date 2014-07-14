using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Validations
{
    public class GetMethodMethodValidator : ReflectionMemberValidatorBase
    {
        public GetMethodMethodValidator(IMethod reflectionMethod, int? bindingFlagArgumentPosition = null) 
            : base(reflectionMethod, CLRDeclaredElementType.METHOD, bindingFlagArgumentPosition)
        {
        }

        protected override bool ProcessAmbigiousMembers(ITypeMember[] ambigious, out ITypeMember resolveMember)
        {
            resolveMember = null;
            return false;
        }

        protected override string GetAmbigiuityResolutionSuggestion()
        {
            return "Use GetMethod overload with argument types specified.";
        }
    }
}
