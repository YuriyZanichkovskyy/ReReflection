using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.IntentionsTests;
using NUnit.Framework;

namespace ReReflection.Tests
{
    [TestFixture]
    public class UseReflectionQuickFixTests : QuickFixTestBase<UseReflectionQuickFix>
    {
        protected override string RelativeTestDataPath
        {
            get { return @"UseReflectionQuickFix"; }
        }

        [Test]
        public void Test01()
        {
            DoTestFiles("execute01.cs");
        }

        [Test]
        public void Test02()
        {
            DoTestFiles("execute02.cs");
        }

        [Test]
        public void Test03()
        {
            DoTestFiles("execute03.cs");
        }
    }
}
