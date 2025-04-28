// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Tests.HookExtension.CommonAttributes;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    internal class ExecutionSequenceWithAllPossibleHooks
    {
        [TestSetupUnderTest]
        public class TestUnderTest
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void Setup()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            [ActivateBeforeTestHooks]
            [ActivateAfterTestHooks]
            public void TestPasses()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            var testResult = TestsUnderTest.Execute();

            Assert.That(testResult.Logs, Is.EqualTo([
                nameof(TestUnderTest.OneTimeSetUp),
                
                nameof(TestUnderTest.Setup),
                HookIdentifiers.BeforeTestHook,
                nameof(TestUnderTest.TestPasses),
                HookIdentifiers.AfterTestHook,

                nameof(TestUnderTest.TearDown),
                nameof(TestUnderTest.OneTimeTearDown)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
