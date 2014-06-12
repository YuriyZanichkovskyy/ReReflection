using System.Linq;
using System.Reflection;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Reflection.Highlightings;

namespace ReSharper.Reflection.Validations
{
    public abstract class ReflectionMemberValidatorBase : ReflectionTypeMethodValidatorBase
    {
        private readonly int? _bindingFlagArgumentPosition;
        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        protected ReflectionMemberValidatorBase(IMethod reflectionMethod, DeclaredElementType expectedMemberType, int? bindingFlagArgumentPosition = null) 
            : base(reflectionMethod)
        {
            _bindingFlagArgumentPosition = bindingFlagArgumentPosition;
            ExpectedMemberType = expectedMemberType;
        }

        public DeclaredElementType ExpectedMemberType { get; private set; }

        public override bool CanValidate(ReflectedTypeResolveResult reflectedType)
        {
            return base.CanValidate(reflectedType) && reflectedType.ResolvedAs != ReflectedTypeResolution.BaseClass;
        }

        public override ReflectionHighlightingBase Validate(ReflectedTypeResolveResult resolvedType, IInvocationExpression invocation)
        {
            
            ITypeMember resolvedMember = null;
            IExpression nameArgument = NameArgument(invocation);

            //We may use TypeElement here as we validate only exact matches
            ITypeMember[] resolvedMembers = ResolveMember(resolvedType.TypeElement, invocation);

            if (resolvedMembers.Length == 0)
            {
                return new ReflectionMemberNotFoundError(nameArgument, ExpectedMemberType, resolvedType.TypeElement);
            }
            if (resolvedMembers.Length > 1)
            {
                if (!ProcessAmbigiousMembers(resolvedMembers, out resolvedMember))
                {
                    return new AmbigiousMemberMatchError(nameArgument, ExpectedMemberType, GetAmbigiuityResolutionSuggestion());
                }
            }
            else
            {
                resolvedMember = resolvedMembers[0];
                return ValidateBindingFlags(resolvedType.TypeElement, resolvedMember, invocation)
                       ?? ValidateCore(resolvedType.TypeElement, resolvedMember, invocation);
            }

            return null;
        }

        protected virtual string GetAmbigiuityResolutionSuggestion()
        {
            return string.Empty;
        }

        protected IExpression NameArgument(IInvocationExpression invocation)
        {
            return invocation.Arguments[0].Expression;
        }

        protected IExpression BindingFlagsArgument(IInvocationExpression invocation)
        {
            if (_bindingFlagArgumentPosition.HasValue)
            {
                return invocation.Arguments[_bindingFlagArgumentPosition.Value].Expression;
            }
            return null;
        }

        protected T? ArgumentConstantValue<T>(IExpression argument)
            where T : struct
        {
            if (argument != null && argument.IsConstantValue() && argument.ConstantValue.Value != null)
            {
                return (T)argument.ConstantValue.Value;
            }

            return null;
        }

        private ITypeMember[] ResolveMember(ITypeElement resolvedType, IInvocationExpression invocation)
        {
            var nameArgument = NameArgument(invocation);
            var bindingFlags = ArgumentConstantValue<BindingFlags>(BindingFlagsArgument(invocation));
            bool ignoreCase = bindingFlags.HasValue && (bindingFlags.Value & BindingFlags.IgnoreCase) != 0;

            if (nameArgument.IsConstantValue())
            {
                string memberName = (string)nameArgument.ConstantValue.Value;
                var members = resolvedType.GetMembers()
                    .Where(m => string.Compare(m.ShortName, memberName, ignoreCase) == 0);
                if (ExpectedMemberType != null)
                {
                    members = members.Where(m => m.GetElementType() == ExpectedMemberType);
                }

                return members.ToArray();
            }

            return new ITypeMember[0];
        }

        private ReflectionHighlightingBase ValidateBindingFlags(ITypeElement resolvedType, ITypeMember resolvedMember, IInvocationExpression invocation)
        {
            var bindingFlagsArgument = BindingFlagsArgument(invocation);
            var expectedMemberFlags = GetMemberExpectedBindingFlags(resolvedMember);

            if (bindingFlagsArgument == null) //argument is not specified
            {
                if ((~DefaultBindingFlags & (expectedMemberFlags ^ DefaultBindingFlags)) != 0)
                {
                    return new IncorrectBindingFlagsError(ReflectionMethod, invocation, null, expectedMemberFlags, resolvedMember);
                }
            }
            else
            {
                var specifiedBindingFlags = ArgumentConstantValue<BindingFlags>(bindingFlagsArgument);
                if (specifiedBindingFlags.HasValue)
                {
                    if ((~specifiedBindingFlags & (expectedMemberFlags ^ specifiedBindingFlags)) != 0)
                    {
                        return new IncorrectBindingFlagsError(ReflectionMethod, invocation, bindingFlagsArgument, expectedMemberFlags, resolvedMember);
                    }
                    if ((~DefaultBindingFlags & (specifiedBindingFlags ^ DefaultBindingFlags)) == 0 && CanSkipBindingFlags())
                    {
                        return new BindingFlagsCanBeSkippedWarning(ReflectionMethod, invocation);
                    }
                }
            }
            return null;
        }

        protected virtual bool CanSkipBindingFlags()
        {
            return true;
        }

        protected BindingFlags GetMemberExpectedBindingFlags(ITypeMember resolvedMember)
        {
            BindingFlags flags = 0;
            if (resolvedMember.IsStatic)
            {
                flags |= BindingFlags.Static;
            }
            else
            {
                flags |= BindingFlags.Instance;
            }
            if (resolvedMember.GetAccessRights() != AccessRights.PUBLIC)
            {
                flags |= BindingFlags.NonPublic;
            }
            else
            {
                flags |= BindingFlags.Public;
            }
            return GetMemberExpectedBindingFlagsAdditional(resolvedMember, flags);
        }

        protected BindingFlags GetMemberExpectedBindingFlagsAdditional(ITypeMember resolvedMember, BindingFlags flags)
        {
            return flags;
        }

        protected virtual ReflectionHighlightingBase ValidateCore(ITypeElement resolvedType, ITypeMember resolvedMember, IInvocationExpression invocation)
        {
            return null;
        }

        protected abstract bool ProcessAmbigiousMembers(ITypeMember[] ambigious, out ITypeMember resolveMember);
    }
}
