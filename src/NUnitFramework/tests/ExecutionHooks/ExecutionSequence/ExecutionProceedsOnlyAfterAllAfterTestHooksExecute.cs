// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence
{
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute
    {
        [Explicit($"This test should only be run as part of the {nameof(TestProceedsAfterAllAfterTestHooksExecute)} test")]
        public class ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest
        {
            [Test]
            [ActivateAfterTestHook]
            [ActivateAfterTestHook]
            [ActivateAfterTestHook]
            [ActivateAfterTestHookThrowingException]
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
            var workItem = TestBuilder.CreateWorkItem(
                typeof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest), TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest.TestPasses),

                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookAttribute),
                nameof(ActivateAfterTestHookThrowingExceptionAttribute),

                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest.TearDown),
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksExecute_TestUnderTest.OneTimeTearDown)
            ]));
        }
    }
}
