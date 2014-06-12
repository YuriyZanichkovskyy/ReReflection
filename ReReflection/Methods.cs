using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.ReSharper.Psi;

namespace ReSharper.Reflection
{
    public static class Methods
    {
        public static MethodInfo Of<T>(Expression<Func<T>> value)
        {
            return GetMethodInfo(value);
        }

        public static bool Is<T>(this IMethod method, Expression<Func<T>> value)
        {
            return GetMethodInfo(value).XmlDocId() == method.XMLDocId;
        }

        private static MethodInfo GetMethodInfo<T>(Expression<Func<T>> value)
        {
            var getMethodInfoVisitor = new GetMethodInfoVisitor();
            getMethodInfoVisitor.Visit(value.Body);
            return getMethodInfoVisitor.MethodInfo;
        }

        private class GetMethodInfoVisitor : ExpressionVisitor
        {
            public MethodInfo MethodInfo;

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type == typeof (MethodInfo))
                {
                    MethodInfo = (MethodInfo)node.Value;
                }
                return node;
            }
        }
    }
}
