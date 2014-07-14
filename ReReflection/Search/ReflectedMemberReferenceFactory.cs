using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReSharper.Reflection.Validations;

namespace ReSharper.Reflection.ReferenceProviders
{
    public class ReflectedMemberReferenceFactory : IReferenceFactory
    {
        public IReference[] GetReferences(ITreeNode element, IReference[] oldReferences)
        {
            if (element is ICSharpArgument)
            {
                var argument = ((ICSharpArgument) element);
                if (argument.Expression != null)
                {
                    var reference = argument.Expression.UserData
                    .GetData(ReflectedMemberReference.Key);
                    if (reference != null)
                    {
                        return new IReference[]
                        {
                            reference
                        };
                    }
                }
            }
            return new IReference[0];
        }

        public bool HasReference(ITreeNode element, ICollection<string> names)
        {
            var argument = element as ICSharpArgument;
            if (argument != null && argument.Expression != null && argument.Expression.IsConstantValue())
            {
                return argument.Expression.ConstantValue.Value != null && names.Contains(argument.Expression.ConstantValue.Value.ToString());
            }
            return false;
        }
    }
}
