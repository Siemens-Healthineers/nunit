// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.TestOutcomeTests;


public class AfterOneTimeSetUpHooksEvaluateTestOutcomeTests
{
    public class AfterSetUpOutcomeLogger : NUnitAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

        public void ApplyToContext(TestExecutionContext context)
        {
            TestResult beforeHookTestResult = null;
            context.ExecutionHooks.BeforeEverySetUp.AddHandler(hookData =>
            {
                beforeHookTestResult = hookData.Context.CurrentResult.Clone();
            });

            context.ExecutionHooks.AfterEverySetUp.AddHandler(hookData =>
            {
                TestResult oneTimeSetUpTestResult
                = hookData.Context.CurrentResult.CalculateDeltaWithPrevious(beforeHookTestResult, hookData.ExceptionContext);

                string outcomeMatchStatement = oneTimeSetUpTestResult.ResultState switch
                {
                    ResultState { Status: TestStatus.Failed } when
                        hookData.Context.CurrentTest.FullName.Contains("4Failed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Passed } when
                        hookData.Context.CurrentTest.FullName.Contains("4Passed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Skipped } when
                        hookData.Context.CurrentTest.FullName.Contains("4Ignored") => OutcomeMatched,
                    ResultState { Status: TestStatus.Inconclusive } when
                        hookData.Context.CurrentTest.FullName.Contains("4Inconclusive") => OutcomeMatched,
                    ResultState { Status: TestStatus.Warning } when
                        hookData.Context.CurrentTest.FullName.Contains("4Warning") => OutcomeMatched,
                    _ => OutcomeMismatch
                };

                TestLog.LogMessage($"{outcomeMatchStatement}: {hookData.Context.CurrentTest.FullName} -> {hookData.Context.CurrentResult.ResultState}");
            });
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

    private static IEnumerable<FailingReason> GetRelevantFailingReasons()
    {
        var failingReasons = Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>();

        // H-ToDo: remove before final checkin
        // Apply filtering
        //failingReasons = failingReasons.Where(reason => reason.ToString().EndsWith("4Warning"));
        return failingReasons;
    }

    [AfterSetUpOutcomeLogger]
    [TestFixtureSource(nameof(GetFixtureConfig))]
    public class TestsUnderTestsWithDifferentOntTimeSetUpOutcome
    {
        private readonly FailingReason _failingReason;

        private static IEnumerable<TestFixtureData> GetFixtureConfig()
        {
            foreach (var failingReason in GetRelevantFailingReasons())
            {
                yield return new TestFixtureData(failingReason);
            }
        }

        public TestsUnderTestsWithDifferentOntTimeSetUpOutcome(FailingReason failingReason)
        {
            _failingReason = failingReason;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ExecuteFailingReason();
        }

        private void ExecuteFailingReason()
        {
            switch (_failingReason)
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
            var fixtureName = TestContext.CurrentContext.Test.Parent.FullName;
            if (!(fixtureName.Contains("4Passed") || fixtureName.Contains("4Warning")))
            {
                TestLog.LogMessage(AfterSetUpOutcomeLogger.OutcomeMismatch +
                            $" -> Test HookedMethod of '{fixtureName}' executed unexpected!");
            }
        }
    }

    [Test]

    public void CheckSetUpOutcomes()
    {
        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithDifferentOntTimeSetUpOutcome));
        workItem.Execute();

        Assert.Multiple(() =>
        {
            Assert.That(TestLog.Logs, Is.Not.Empty);
            foreach (string log in TestLog.Logs)
            {
                Assert.That(log, Does.Not.Contain(AfterSetUpOutcomeLogger.OutcomeMismatch));
            }

            Assert.That(workItem.Result.PassCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Passed") || reason.ToString().EndsWith("4Warning"))));
            Assert.That(workItem.Result.FailCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Failed"))));
            Assert.That(workItem.Result.SkipCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(workItem.Result.TotalCount, Is.EqualTo(GetRelevantFailingReasons().Count()));
        });

        TestLog.Logs.Clear();
    }
}
