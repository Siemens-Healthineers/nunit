// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class BeforeTestHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterBeforeTestHookCompletes)} test")]
        public class TestWithBeforeTestHookOnMethod
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void SetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            [ActivateBeforeTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterBeforeTestHookCompletes()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithBeforeTestHookOnMethod), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestWithBeforeTestHookOnMethod.OneTimeSetUp),
                nameof(TestWithBeforeTestHookOnMethod.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithBeforeTestHookOnMethod.EmptyTest),
                nameof(TestWithBeforeTestHookOnMethod.TearDown),
                nameof(TestWithBeforeTestHookOnMethod.OneTimeTearDown)
            ]));
        }
    }
}
