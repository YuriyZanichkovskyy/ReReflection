using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Intentions.Test;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;

namespace ReReflection.Tests
{
    [TestFixture]
    public class UseReflectionQuickFixAvailabiityTests : QuickFixAvailabilityTestBase
    {
        protected override string RelativeTestDataPath
        {
            get { return @"UseReflectionQuickFix"; }
        }

        protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile psiSourceFile)
        {
            return highlighting is AccessRightsError;
        }

        [Test]
        public void Availability01()
        {           
            DoTestFiles("availability01.cs");
        }

        [Test]
        public void Availability02()
        {
            DoTestFiles("availability02.cs");
        }
    }
}
