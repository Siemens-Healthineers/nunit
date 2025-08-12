// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.ExecutionHooks;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterOneTimeSetUpHooksEvaluateTestOutcomeTests
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AfterSetUpOutcomeLoggerAttribute : ExecutionHookAttribute
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";
        private TestContext.ResultAdapter? _beforeHookTestResult;

        public override void BeforeEverySetUpHook(HookData hookData)
        {
            _beforeHookTestResult = hookData.Context.Result.Clone();
        }

        public override void AfterEverySetUpHook(HookData hookData)
        {
            Assert.That(_beforeHookTestResult, Is.Not.Null, "BeforeEverySetUp was not called before AfterEverySetUp.");

            if (_beforeHookTestResult is null)
            {
                return;
            }

            var oneTimeSetUpTestResult =
                hookData.Context.Result.CalculateDeltaWithPrevious(_beforeHookTestResult, hookData.Exception);
            var outcomeMatchStatement = oneTimeSetUpTestResult.Outcome switch
            {
                { Status: TestStatus.Failed } when
                    hookData.Context.Test.FullName.Contains("4Failed") => OutcomeMatched,
                { Status: TestStatus.Passed } when
                    hookData.Context.Test.FullName.Contains("4Passed") => OutcomeMatched,
                { Status: TestStatus.Skipped } when
                    hookData.Context.Test.FullName.Contains("4Ignored") => OutcomeMatched,
                { Status: TestStatus.Inconclusive } when
                    hookData.Context.Test.FullName.Contains("4Inconclusive") => OutcomeMatched,
                { Status: TestStatus.Warning } when
                    hookData.Context.Test.FullName.Contains("4Warning") => OutcomeMatched,
                _ => OutcomeMismatch
            };

            TestLog.LogMessage(
                $"{outcomeMatchStatement}: {hookData.Context.Test.FullName} -> {hookData.Context.Result.Outcome}");
        }
    }

    public enum FailingReason
    {
        Assertion4Failed,
        MultiAssertion4Failed,
        Exception4Failed,
        IgnoreAssertion4Ignored,
        IgnoreException4Ignored,
        Inconclusive4Inconclusive,
        Warning4Warning, // Warn counts on OneTimeSetUp level as passed and on SetUp level as warning!
        None4Passed
    }

    [Explicit($"This test should only be run as part of the {nameof(CheckSetUpOutcomes)} test")]
    [AfterSetUpOutcomeLogger]
    [TestFixtureSource(nameof(GetReasonsToFail))]
    public class TestsUnderTestsWithDifferentOneTimeSetUpOutcome(FailingReason failingReason)
    {
        private static IEnumerable<TestFixtureData> GetReasonsToFail()
        {
            return Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().Select(failingReason => new TestFixtureData(failingReason));
        }

        [OneTimeSetUp]
        public void OneTimeSetUp() => ExecuteFailingReason();

        private void ExecuteFailingReason()
        {
            switch (failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("OneTimeSetUp fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st OneTimeSetUp fails by MultiAssertion_Failed.");
                        Assert.Fail("2nd OneTimeSetUp fails by MultiAssertion_Failed.");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("OneTimeSetUp throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("OneTimeSetUp ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("OneTimeSetUp ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("OneTimeSetUp is inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("OneTimeSetUp with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SomeTest()
        {
            Assert.That(TestContext.CurrentContext.Test.Parent, Is.Not.Null);

            var fixtureName = TestContext.CurrentContext.Test.Parent.FullName;
            if (!(fixtureName.Contains("4Passed") || fixtureName.Contains("4Warning")))
            {
                TestLog.LogMessage(AfterSetUpOutcomeLoggerAttribute.OutcomeMismatch +
                            $" -> Test HookedMethod of '{fixtureName}' executed unexpected!");
            }
        }
    }

    [Test]

    public void CheckSetUpOutcomes()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithDifferentOneTimeSetUpOutcome), TestFilter.Explicit);
        workItem.Execute();
        var currentTestLogs = TestLog.Logs(workItem.Test);

        Assert.That(currentTestLogs, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var log in currentTestLogs)
            {
                Assert.That(log, Does.Not.Contain(AfterSetUpOutcomeLoggerAttribute.OutcomeMismatch));
            }

            var failingReasons = Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>().ToList();
            Assert.That(workItem.Result.PassCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Passed") || reason.ToString().EndsWith("4Warning"))));
            Assert.That(workItem.Result.FailCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Failed"))));
            Assert.That(workItem.Result.SkipCount, Is.EqualTo(failingReasons.Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(workItem.Result.TotalCount, Is.EqualTo(failingReasons.Count));
        });
    }
}
