using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp;
using NUnit.Framework;
using ReSharper.Reflection.Highlightings;

namespace ReReflection.Tests
{
    [TestFixture]
    public class ReflectionProblemAnalyzersTests : CSharpHighlightingTestBase
    {
        protected override bool HighlightingPredicate(IHighlighting highlighting, IContextBoundSettingsStore settingsStore)
        {
            return highlighting is ReflectionHighlightingBase;
        }

        protected override string RelativeTestDataPath
        {
            get { return "ReflectionProblemsAnalyzer"; }
        }

        [Test]
        public void Test01()
        {
            //For different cases where member is not present in type
            DoTestFiles("Case1.cs");
        }

        [Test]
        public void Test02()
        {
            //Make generic validations
            DoTestFiles("Case2.cs");
        }

        [Test]
        public void Test03()
        {
            //BindingFlags verification
            DoTestFiles("Case3.cs");
        }

        [Test]
        public void Test04()
        {
            DoTestFiles("Case4.cs");
        }

        [Test]
        public void Test05()
        {
            DoTestFiles("Case5.cs");
        }

        [Test]
        public void Test06()
        {
            DoTestFiles("Case6.cs");
        }
    }
}
