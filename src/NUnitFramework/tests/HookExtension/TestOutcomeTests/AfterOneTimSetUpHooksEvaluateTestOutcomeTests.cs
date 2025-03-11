// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;
using TestResult = NUnit.Framework.Internal.TestResult;

namespace NUnit.Framework.Tests.HookExtension.TestOutcomeTests;


public class AfterOneTimeSetUpHooksEvaluateTestOutcomeTests
{
    public class AfterSetUpOutcomeLogger : NUnitAttribute, IApplyToContext
    {
        internal static readonly string OutcomeMatched = "Outcome Matched";
        internal static readonly string OutcomeMismatch = "Outcome Mismatch!!!";

        public void ApplyToContext(TestExecutionContext context)
        {
            TestResult beforeHookTestResult = null;
            context.HookExtension?.BeforeAnySetUps.AddHandler((sender, eventArgs) =>
            {
                beforeHookTestResult = eventArgs.Context.CurrentResult.Clone();
            });

            context.HookExtension?.AfterAnySetUps.AddHandler((sender, eventArgs) =>
            {
                TestResult oneTimeSetUpTestResult 
                = eventArgs.Context.CurrentResult.CalculateDeltaWithPrevious(beforeHookTestResult, eventArgs.ExceptionContext);

                string outcomeMatchStatement = oneTimeSetUpTestResult.ResultState switch
                {
                    ResultState { Status: TestStatus.Failed } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Failed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Passed } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Passed") => OutcomeMatched,
                    ResultState { Status: TestStatus.Skipped } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Ignored") => OutcomeMatched,
                    ResultState { Status: TestStatus.Inconclusive } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Inconclusive") => OutcomeMatched,
                    ResultState { Status: TestStatus.Warning } when
                        eventArgs.Context.CurrentTest.FullName.Contains("4Warning") => OutcomeMatched,
                    _ => OutcomeMismatch
                };

                TestLog.Log($"{outcomeMatchStatement}: {eventArgs.Context.CurrentTest.FullName} -> {eventArgs.Context.CurrentResult.ResultState}");
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

    [TestSetupUnderTest]
    [NonParallelizable]
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
                TestLog.Log(AfterSetUpOutcomeLogger.OutcomeMismatch +
                            $" -> Test Method of '{fixtureName}' executed unexpected!");
            }
        }
    }

    [Test]
    [NonParallelizable]
    public void CheckSetUpOutcomes()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.Multiple(() =>
        {
            foreach (string log in testResult.Logs)
            {
                Assert.That(log, Does.Not.Contain(AfterSetUpOutcomeLogger.OutcomeMismatch));
            }

            foreach (TestCase testCase in testResult.TestRunResult.TestCases)
            {
                string resultStatusPartOfTestName = testCase.FullName.Split('4').Last();

                string expectedResult = resultStatusPartOfTestName switch
                {
                    "Ignored" => "Skipped",
                    "Warning" => "Passed", // Only for OneTimeSetUp level!
                    _ => resultStatusPartOfTestName
                };

                Assert.That(testCase.FullName, Does.Contain(expectedResult));
            }

            Assert.That(testResult.TestRunResult.Passed, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Passed") || reason.ToString().EndsWith("4Warning"))));
            Assert.That(testResult.TestRunResult.Failed, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Failed"))));
            Assert.That(testResult.TestRunResult.Skipped, Is.EqualTo(GetRelevantFailingReasons().Count(reason => reason.ToString().EndsWith("4Ignored"))));
            Assert.That(testResult.TestRunResult.Total, Is.EqualTo(GetRelevantFailingReasons().Count()));
        });

        TestLog.Logs.Clear();
    }
}
