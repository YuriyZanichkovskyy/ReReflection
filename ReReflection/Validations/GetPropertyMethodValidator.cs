using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection.Validations
{
    internal class GetPropertyMethodValidator : ReflectionMemberValidatorBase
    {
        public GetPropertyMethodValidator(IMethod reflectionMethod, int? bindingFlagArgumentPosition = null)
            : base(reflectionMethod, CLRDeclaredElementType.PROPERTY, bindingFlagArgumentPosition)
        {
        }

        protected override bool ProcessAmbigiousMembers(ITypeMember[] ambigious, out ITypeMember resolveMember)
        {
            //Ambigiuity might occur only for indexers.
            resolveMember = null;
            return false;
        }
    }
}
