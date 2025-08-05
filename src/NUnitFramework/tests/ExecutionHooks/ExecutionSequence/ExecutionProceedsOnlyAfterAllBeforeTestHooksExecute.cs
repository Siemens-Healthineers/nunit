// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.ExecutionSequence;

public class ExecutionProceedsOnlyAfterAllBeforeTestHooksExecute
{
    [Explicit($"This test should only be run as part of the {nameof(CheckThatLongRunningBeforeTestHooksCompleteBeforeTest)} test")]
    private sealed class TestUnderTest
    {
        [Test]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateBeforeTestHook]
        [ActivateLongRunningBeforeTestHook]
        public void SomeTest()
        {
            TestLog.LogCurrentMethod();
        }
    }

    [Test]
    public void CheckThatLongRunningBeforeTestHooksCompleteBeforeTest()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest), TestFilter.Explicit);
        workItem.Execute();
        var testLogs = TestLog.FetchLogsForTest(workItem.Test);

        Assert.That(testLogs, Is.EqualTo([
            nameof(ActivateLongRunningBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(ActivateBeforeTestHookAttribute),
            nameof(TestUnderTest.SomeTest)
        ]));

        TestLog.Clear();
    }
}
