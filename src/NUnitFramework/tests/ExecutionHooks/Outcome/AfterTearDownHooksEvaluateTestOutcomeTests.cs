// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.ExecutionHooks.Outcome;

public class AfterTearDownHooksEvaluateTestOutcomeTests
{
    public class AfterTearDownOutcomeLogger : NUnitAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

        public void ApplyToContext(TestExecutionContext context)
        {
            TestResult? beforeHookTestResult = null;
            context.ExecutionHooks.BeforeEveryTearDown.AddHandler((hookData) =>
            {
                beforeHookTestResult = hookData.Context.CurrentResult.Clone();
            });

            context.ExecutionHooks.AfterEveryTearDown.AddHandler((hookData) =>
            {
                Assert.That(beforeHookTestResult, Is.Not.Null, "BeforeEveryTearDown was not called before AfterEveryTearDown.");

                TestResult tearDownTestResult
                    = hookData.Context.CurrentResult.CalculateDeltaWithPrevious(beforeHookTestResult, hookData.ExceptionContext);

                string outcomeMatchStatement = tearDownTestResult.ResultState switch
                {
                    ResultState { Status: TestStatus.Failed } when hookData.Context.CurrentTest.FullName.Contains("4Failed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Passed } when hookData.Context.CurrentTest.FullName.Contains("4Passed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Skipped } when hookData.Context.CurrentTest.FullName.Contains("4Ignored") => OutcomeMatched,
                    ResultState { Status: TestStatus.Inconclusive } when hookData.Context.CurrentTest.FullName.Contains("4Inconclusive") => OutcomeMatched,
                    ResultState { Status: TestStatus.Warning } when hookData.Context.CurrentTest.FullName.Contains("4Warning") => OutcomeMatched,
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
        Warning4Warning, // Warn counts on OneTimeTearDown level as passed and on TearDown level as warning!
        None4Passed
    }

    private static IEnumerable<FailingReason> GetRelevantFailingReasons()
    {
        var failingReasons = Enum.GetValues(typeof(FailingReason)).Cast<FailingReason>();

        // H-ToDo: remove before final checkin
        // Apply filtering
        //failingReasons = failingReasons.Where(reason => !reason.ToString().EndsWith("4Inconclusive"));
        return failingReasons;
    }

    // H-TODO: enrich the test to also failing tests and setups
    [Explicit($"This test should only be run as part of the {nameof(CheckTearDownOutcomes)} test")]
    [AfterTearDownOutcomeLogger]
    [TestFixtureSource(nameof(GetFixtureConfig))]
    public class TestsUnderTestsWithDifferentTearDownOutcome(FailingReason failingReason)
    {
        private static IEnumerable<TestFixtureData> GetFixtureConfig()
        {
            foreach (var failingReason in GetRelevantFailingReasons())
            {
                yield return new TestFixtureData(failingReason);
            }
        }
        
        [TearDown]
        public void TearDown()
        {
            ExecuteFailingReason();
        }

        private void ExecuteFailingReason()
        {
            switch (failingReason)
            {
                case FailingReason.Assertion4Failed:
                    Assert.Fail("TearDown fails by Assertion_Failed.");
                    break;
                case FailingReason.MultiAssertion4Failed:
                    Assert.Multiple(() =>
                    {
                        Assert.Fail("1st failure");
                        Assert.Fail("2nd failure");
                    });
                    break;
                case FailingReason.Exception4Failed:
                    throw new Exception("TearDown throwing an exception.");
                case FailingReason.None4Passed:
                    break;
                case FailingReason.IgnoreAssertion4Ignored:
                    Assert.Ignore("TearDown ignored by Assert.Ignore.");
                    break;
                case FailingReason.IgnoreException4Ignored:
                    throw new IgnoreException("TearDown ignored by IgnoreException.");
                case FailingReason.Inconclusive4Inconclusive:
                    Assert.Inconclusive("TearDown ignored by Assert.Inconclusive.");
                    break;
                case FailingReason.Warning4Warning:
                    Assert.Warn("TearDown with warning.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Test]
        public void SomeTest()
        {
        }
    }

    [Test]
    public void CheckTearDownOutcomes()
    {
        TestLog.Clear();

        var workItem = TestBuilder.CreateWorkItem(typeof(TestsUnderTestsWithDifferentTearDownOutcome), TestFilter.Explicit);
        workItem.Execute();

        Assert.Multiple(() =>
        {
            Assert.That(TestLog.Logs, Is.Not.Empty);
            foreach (string log in TestLog.Logs)
            {
                Assert.That(log, Does.Not.Contain(AfterTearDownOutcomeLogger.OutcomeMismatch));
            }

            Assert.That(workItem.Result.PassCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Passed"))));
            Assert.That(workItem.Result.FailCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Failed"))));
            // H-TODO: Understand the change in the test outcome. Find the relevant nunit issue for that!
            Assert.That(workItem.Result.SkipCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(workItem.Result.InconclusiveCount, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Inconclusive"))));
            Assert.That(workItem.Result.TotalCount, Is.EqualTo(GetRelevantFailingReasons().Count()));
        });

        TestLog.Clear();
    }
}
