// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class CombinedHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterBothTestHookCompletes)} test")]
        public class TestWithNormalAndLongRunningTestHooks
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
            [ActivateLongRunningBeforeTestHook]
            [ActivateAfterTestHook]
            [ActivateLongRunningAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterBothTestHookCompletes()
        {
            // Capture current context logs reference
            //var currentTestLogs = TestLog.Logs;
            //currentTestLogs.Clear();

            var workItem =
                TestBuilder.CreateWorkItem(typeof(TestWithNormalAndLongRunningTestHooks), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestWithNormalAndLongRunningTestHooks.OneTimeSetUp),
                nameof(TestWithNormalAndLongRunningTestHooks.SetUp),

                nameof(ActivateLongRunningBeforeTestHookAttribute),
                nameof(ActivateBeforeTestHookAttribute),

                nameof(TestWithNormalAndLongRunningTestHooks.EmptyTest),

                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateLongRunningAfterTestHookAttribute),

                nameof(TestWithNormalAndLongRunningTestHooks.TearDown),
                nameof(TestWithNormalAndLongRunningTestHooks.OneTimeTearDown)
            ]));
        }
    }
}
