using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;

namespace ReSharper.Reflection.Services
{
    public static class ReflectedTypeHelper
    {
        public static ReflectedTypeResolveResult ResolveReflectedType(IInvocationExpression invocationExpression)
        {
            //var finder = invocationExpression.GetPsiServices().Finder;
            //finder.FindReferences();

            var referenceExpression = invocationExpression.InvokedExpression as IReferenceExpression;

            if (referenceExpression != null)
            {
                var typeOfExpression = referenceExpression.QualifierExpression as ITypeofExpression;
                if (typeOfExpression != null)
                {
                    var type = typeOfExpression.ArgumentType.GetTypeElement<ITypeElement>();
                    if (type == null)
                    {
                        return ReflectedTypeResolveResult.NotResolved;
                    }

                    return new ReflectedTypeResolveResult(type, ReflectedTypeResolution.Exact);
                }

                //GetType, MakeArrayType, 
                var methodInvocationExpression = referenceExpression.QualifierExpression as IInvocationExpression;
                if (methodInvocationExpression != null && IsReflectionTypeMethod(invocationExpression, "MakeGenericType"))
                {
                    var resolvedType = ResolveReflectedType(methodInvocationExpression);
                    if (resolvedType.ResolvedAs == ReflectedTypeResolution.Exact)
                    {
                        return new ReflectedTypeResolveResult(resolvedType.TypeElement, ReflectedTypeResolution.ExactMakeGeneric);
                    }
                }

                //var typeReference = referenceExpression.QualifierExpression as IReferenceExpression;
                //if (typeReference != null)
                //{
                    
                //}
            }

            return ReflectedTypeResolveResult.NotResolved;
        }

        public static bool IsReflectionTypeMethod(IInvocationExpression expression, string methodName)
        {
            IMethod method;
            return IsReflectionTypeMethod(expression, true, out method) && method.ShortName == methodName;
        }

        public static bool IsReflectionTypeMethod(IInvocationExpression expression, out IMethod method)
        {
            return IsReflectionTypeMethod(expression, true, out method);
        }

        //exact false if we are interested only in method name, but not exact method being invoked
        public static bool IsReflectionTypeMethod(IInvocationExpression expression, bool exact, out IMethod method)
        {
            var reference = expression.InvocationExpressionReference;
            var resolveResult = reference.Resolve();
            if (resolveResult.ResolveErrorType == ResolveErrorType.OK)
            {
                method = resolveResult.DeclaredElement as IMethod;

                if (method != null && GetContainingTypeName(method) == "System.Type")
                    return true;
            }
            if (!exact)
            {
                method = resolveResult.Result.Candidates.FirstOrDefault() as IMethod;

                if (method != null && GetContainingTypeName(method) == "System.Type")
                    return true;
            }

            method = null;
            return false;
        }

        public static string GetContainingTypeName(IMethod method)
        {
            var type = method.GetContainingType();
            return type == null ? null : type.GetClrName().FullName;
        }
    }
}
