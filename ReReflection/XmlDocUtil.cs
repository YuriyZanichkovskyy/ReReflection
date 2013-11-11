using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace ReReflection
{
    public static class XmlDocUtil
    {
        [NotNull]
        public static string XmlDocId(this MemberInfo element)
        {
            if (element == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            if (element is EventInfo)
                builder.Append("E:");
            else if (element is FieldInfo)
                builder.Append("F:");
            else if (element is MethodInfo)
            {
                builder.Append("M:");
            }
            else
            {
                if (!(element is PropertyInfo))
                    return string.Empty;
                builder.Append("P:");
            }

            builder.Append(GetTypeElementQualifiedName(element.DeclaringType));
            builder.Append(".");
            builder.Append(element.Name);
            
            if (element is MethodInfo)
            {
                if (((MethodInfo)element).IsGenericMethod)
                {
                    builder.Append("``");
                    builder.Append(((MethodInfo)element).GetGenericArguments().Length);
                }
            }
            if (element is MethodBase)
            {
                var methodBase = (MethodBase)element;
                var parameters = methodBase.GetParameters();
                if (parameters.Length > 0)
                {
                    builder.Append("(");
                    for (int index = 0; index < parameters.Length; ++index)
                    {
                        var parameter = parameters[index];
                        BuildTypeString(builder, parameter.ParameterType);
                        if (parameter.IsOut || parameter.ParameterType.IsByRef)
                            builder.Append("@");
                        if (index < parameters.Length - 1)
                            builder.Append(",");
                    }
                    builder.Append(")");
                }
            }
            return builder.ToString();
        }

        public static string GetTypeElementXmlDocId(Type element)
        {
            if (element == null)
                return string.Empty;
            else
                return "T:" + GetTypeElementQualifiedName(element);
        }

        private static string GetTypeElementQualifiedName(Type element)
        {
            return element.FullName.Replace('+', '.');
        }

        private static void BuildTypeString(StringBuilder builder, Type type)
        {
            if (type.IsPointer)
            {
                BuildTypeString(builder, type.GetElementType());
                builder.Append("*");
            }
            else
            {
                if (type.IsArray)
                {
                    BuildTypeString(builder, type.GetElementType());
                    BuildArrayRankString(builder, type);
                }
                else
                {
                    builder.Append(GetTypeElementQualifiedName(type));
                }
            }
        }

        private static void BuildArrayRankString(StringBuilder builder, Type arrayType)
        {
            builder.Append("[");
            int rank = arrayType.GetArrayRank();
            if (rank > 1)
            {
                for (int index = 0; index < rank; ++index)
                {
                    builder.Append("0:");
                    if (index < rank - 1)
                        builder.Append(",");
                }
            }
            builder.Append("]");
        }
    }
}
