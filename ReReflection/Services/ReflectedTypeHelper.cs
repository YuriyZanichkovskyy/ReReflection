using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Search;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReSharper.Reflection.Services
{
    public static class ReflectedTypeHelper
    {
        private const int MaxResolutionRecursion = 5;

        public static ReflectedTypeResolveResult ResolveReflectedType(IInvocationExpression invocationExpression)
        {                      
            var referenceExpression = invocationExpression.InvokedExpression as IReferenceExpression;

            if (referenceExpression != null)
            {
                return ResolveReflectedTypeInternal(referenceExpression.QualifierExpression);
            }

            return ReflectedTypeResolveResult.NotResolved;
        }

        private static ReflectedTypeResolveResult ResolveReflectedTypeInternal(ICSharpExpression expression, int recursion = 0)
        {
            if (MaxResolutionRecursion == recursion)
                return ReflectedTypeResolveResult.NotResolved;

            var typeOfExpression = expression as ITypeofExpression;
            if (typeOfExpression != null)
            {
                var type = typeOfExpression.ArgumentType.GetTypeElement<ITypeElement>();
                if (type == null)
                {
                    return ReflectedTypeResolveResult.NotResolved;
                }

                return new ReflectedTypeResolveResult(typeOfExpression.ArgumentType, ReflectedTypeResolution.Exact);
            }

            var methodInvocationExpression = expression as IInvocationExpression;
            if (methodInvocationExpression != null)
            {
                if (IsReflectionTypeMethod(methodInvocationExpression, "MakeGenericType"))
                {
                    var resolvedType = ResolveReflectedType(methodInvocationExpression);
                    if (resolvedType.ResolvedAs == ReflectedTypeResolution.Exact)
                    {
                        IType[] parameters;
                        if (GetTypeParameters(methodInvocationExpression, out parameters))
                        {
                            return new ReflectedTypeResolveResult(TypeFactory.CreateType(resolvedType.TypeElement, parameters), 
                                ReflectedTypeResolution.ExactMakeGeneric);
                        }
                    }
                }
                else if (IsReflectionTypeMethod(methodInvocationExpression, "GetType"))
                {
                    var qualifier = ((IReferenceExpression) methodInvocationExpression.InvokedExpression).QualifierExpression;
                    if (qualifier != null)
                    {
// ReSharper disable once AssignNullToNotNullAttribute
                        return new ReflectedTypeResolveResult(qualifier.GetExpressionType().ToIType(), ReflectedTypeResolution.BaseClass);
                    }
                }
                else if (IsReflectionTypeMethod(methodInvocationExpression, "MakeArrayType"))
                {
                    var resolvedType = ResolveReflectedType(methodInvocationExpression);
                    if (resolvedType.ResolvedAs != ReflectedTypeResolution.NotResolved)
                    {
                        return new ReflectedTypeResolveResult(TypeFactory.CreateArrayType(resolvedType.Type, 1), ReflectedTypeResolution.Exact); 
                    }
                }
            }

            var reference = expression as IReferenceExpression;
            if (reference != null)
            {
                var resolveResult = reference.Reference.Resolve();
                if (resolveResult.ResolveErrorType == ResolveErrorType.OK && resolveResult.DeclaredElement != null)
                {
                    //
                    var assignmentExpressions = new List<ICSharpExpression>();
                    var declarations = resolveResult.DeclaredElement.GetDeclarations().OfType<IInitializerOwnerDeclaration>().ToArray();
                    if (declarations.Length > 0)
                    {
                        var expressionInitializer = (IExpressionInitializer)declarations[0].Initializer;
                        if (expressionInitializer != null)
                            assignmentExpressions.Add(expressionInitializer.Value);
                    }

                    var findResultConsumer = new FindResultConsumer(result =>
                    {
                        var resultReference = result as IFindResultReference;
                        if (resultReference != null)
                        {
                            var assignment = resultReference.Reference.GetTreeNode().GetContainingNode<IAssignmentExpression>();
                            if (assignment != null)
                            {
                                assignmentExpressions.Add(assignment.Source);
                            }
                        }
                        return FindExecution.Continue;
                    });


                    var finder = expression.GetPsiServices().Finder;
                    finder.FindReferences(resolveResult.DeclaredElement,
                        resolveResult.DeclaredElement.GetSearchDomain(),
                        findResultConsumer, NullProgressIndicator.Instance);

                    if (assignmentExpressions.Count == 1)
                    {
                        return ResolveReflectedTypeInternal(assignmentExpressions[0], recursion + 1);
                        //Try to resolve appropriate reference
                    }
                }
            }

            return ReflectedTypeResolveResult.NotResolved;
        }

        private static bool GetTypeParameters(IInvocationExpression invocation, out IType[] typeParameters)
        {
            typeParameters = new IType[0];
            if (invocation.Arguments.Count != 0)
            {
                ICSharpExpression[] expressions;
                if (invocation.Arguments[0].Expression is IArrayCreationExpression)
                {
                    var initiallizer = ((IArrayCreationExpression) invocation.Arguments[0].Expression).ArrayInitializer;
                    if (initiallizer != null)
                    {
                        expressions = initiallizer.ElementInitializers
                            .Cast<IExpressionInitializer>()
                            .Select(i => i.Value)
                            .ToArray();
                    }
                    else
                    {
                        expressions = new ICSharpExpression[0];
                    }
                }
                else
                {
                    expressions = invocation.Arguments.Select(a => a.Expression)
                        .Cast<ICSharpExpression>()
                        .ToArray();
                }

                typeParameters = new IType[expressions.Length];

                for (int i = 0; i < expressions.Length; i++)
                {
                    var resolvedType = ResolveReflectedTypeInternal(expressions[i]);
                    if (resolvedType.ResolvedAs == ReflectedTypeResolution.Exact)
                    {
                        typeParameters[i] = resolvedType.Type;
                    }
                    else
                    {
                        return false; //
                    }
                }
            }

            return true;
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
