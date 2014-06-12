using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.Tests.CSharp.FeatureServices.CodeCompletion;
using NUnit.Framework;
using ReSharper.Reflection.Completion;

namespace ReReflection.Tests
{
    [TestFixture]
    public class ReflectionCodeCompletionTests : CodeCompletionTestBase
    {
        protected override IEnumerable<ILookupItem> GetItemsFromResult(ICodeCompletionResult result, JetHashSet<ILookupItem> best)
        {
            return base.GetItemsFromResult(result, best).OfType<ReflectionMemberLookupItem>();
        }

        protected override bool ExecuteAction
        {
            get
            {
                return true;
            }
        }

        protected override string RelativeTestDataPath
        {
            get
            {
                return "ReflectionAutoCompletion";
            }
        }

        [Test]
        public void Test01()
        {
            DoTestFiles("Test01.cs");
        }
    }
}
