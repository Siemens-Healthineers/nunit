// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterTestHooksEvaluateTestOutcomeTests
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AfterTestOutcomeLoggerAttribute : ExecutionHookAttribute
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        private TestContext.ResultAdapter? _beforeHookTestResult;

        public override void BeforeTestHook(HookData hookData)
        {
            _beforeHookTestResult = hookData.Context.Result.Clone();
        }

        public override void AfterTestHook(HookData hookData)
        {
            Assert.That(_beforeHookTestResult, Is.Not.Null, "BeforeTestHook was not called before AfterTestHook.");
            Assert.That(hookData.Context.Test.MethodName, Is.Not.Null, "Hook was not called on a method.");

            TestContext.ResultAdapter testResult
                    = hookData.Context.Result.CalculateDeltaWithPrevious(_beforeHookTestResult, hookData.Exception);

            string outcomeMatchStatement = testResult.Outcome switch
            {
                { Status: TestStatus.Failed } when
                    hookData.Context.Test.MethodName.StartsWith("FailedTest") => OutcomeMatched,
                { Status: TestStatus.Passed } when
                    hookData.Context.Test.MethodName.StartsWith("PassedTest") => OutcomeMatched,
                { Status: TestStatus.Skipped } when
                    hookData.Context.Test.MethodName.StartsWith("TestIgnored") => OutcomeMatched,
                { Status: TestStatus.Warning } when
                    hookData.Context.Test.MethodName.StartsWith("WarningTest") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.LogMessage(
                $"{outcomeMatchStatement}: {hookData.Context.Test.MethodName} -> {hookData.Context.Result.Outcome}");
        }
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckThatAfterTestHooksEvaluateTestOutcome)} test")]
    [AfterTestOutcomeLogger]
    [TestFixture]
    public class TestsUnderTestsWithMixedOutcome
    {
        [Test]
        public void PassedTest()
        {
        }

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
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithMixedOutcome), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.That(currentTestLogs, Has.All.StartWith(AfterTestOutcomeLoggerAttribute.OutcomeMatched));
    }
}
