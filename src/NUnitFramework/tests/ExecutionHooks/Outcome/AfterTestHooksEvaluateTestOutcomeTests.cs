// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterTestHooksEvaluateTestOutcomeTests
{
    public class AfterTestOutcomeLogger : ExecutionHookAttribute
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        TestResult beforeHookTestResult = null;

        public override void BeforeTestHook(HookData hookData)
        {
            beforeHookTestResult = hookData.Context.CurrentResult.Clone();
        }

        public override void AfterTestHook(HookData hookData)
        {
            TestResult testResult
                    = hookData.Context.CurrentResult.CalculateDeltaWithPrevious(beforeHookTestResult, hookData.ExceptionContext);

            string outcomeMatchStatement = testResult.ResultState switch
            {
                ResultState { Status: TestStatus.Failed } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("FailedTest") => OutcomeMatched,
                ResultState { Status: TestStatus.Passed } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("PassedTest") => OutcomeMatched,
                ResultState { Status: TestStatus.Skipped } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("TestIgnored") => OutcomeMatched,
                ResultState { Status: TestStatus.Warning } when
                    hookData.Context.CurrentTest.MethodName.StartsWith("WarningTest") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.LogMessage(
                $"{outcomeMatchStatement}: {hookData.Context.CurrentTest.MethodName} -> {hookData.Context.CurrentResult.ResultState}");
        }
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckThatAfterTestHooksEvaluateTestOutcome)} test")]
    [AfterTestOutcomeLogger]
    [TestFixture]
    public class TestsUnderTestsWithMixedOutcome
    {
        [Test]
        public void PassedTest() { }

        [Test]
        public void FailedTestByAssertion()
        {
            Assert.Fail();
        }

        [Test]
        public void FailedTestByException()
        {
            throw new System.Exception("some exception");
        }

        [TestCase(ExpectedResult = 1)]
        public int FailedTestByWrongExpectedResult() => 2;

        [Test]
        public void TestIgnoredByAssertIgnore()
        {
            Assert.Ignore();
        }

        [Test]
        public void TestIgnoredByException()
        {
            throw new IgnoreException("Ignore this test");
        }

        [Test]
        public void WarningTestWithWarnings()
        {
            Assert.Warn("Some warning.");
        }
    }

    [Test]
    public void CheckThatAfterTestHooksEvaluateTestOutcome()
    {
        TestLog.Clear();

        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithMixedOutcome), TestFilter.Explicit);
        workItem.Execute();

        Assert.That(TestLog.Logs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (string logLine in TestLog.Logs)
            {
                Assert.That(logLine, Does.StartWith(AfterTestOutcomeLogger.OutcomeMatched));
            }
        });

        TestLog.Clear();
    }
}
