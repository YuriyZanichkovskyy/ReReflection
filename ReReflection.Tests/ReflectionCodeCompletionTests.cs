using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.Tests.CSharp.FeatureServices.CodeCompletion;

namespace ReReflection.Tests
{
    public class ReflectionCodeCompletionTests : CodeCompletionTestBase
    {
       
        protected override bool ExecuteAction
        {
            get
            {
                return true;
            }
        }
    }
}
