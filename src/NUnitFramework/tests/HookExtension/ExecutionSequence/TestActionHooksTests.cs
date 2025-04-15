using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence;

public class TestActionLoggingHookExtension : NUnitAttribute, IApplyToContext
{
    public void ApplyToContext(TestExecutionContext context)
    {
        context.HookExtension?.BeforeTestAction.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"BeforeTestAction({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.AfterTestAction.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"AfterTestAction({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
    }
}

public class LogTestActionAttribute : Attribute, ITestAction
{
    public void BeforeTest(ITest test)
    {
        TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
    }

    public void AfterTest(ITest test)
    {
        TestLog.LogCurrentMethodWithContextInfo(test.IsSuite ? "Suite" : "Test");
    }

    public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;
}

// H-ToDo: enrich test by Hooks and implement

public class TestActionHooksTests
{
    [LogTestAction]
    [TestActionLoggingHookExtension]
    [TestSetupUnderTest]
    public class TestClassWithTestAction
    {
        [Test]
        public void TestUnderTest() => TestLog.LogCurrentMethod();
    }

    [Test]
    public void CheckTestLogs()
    {
        var testResult = TestsUnderTest.Execute();

        Assert.That(testResult.Logs, Is.EqualTo([
            "BeforeTestAction(Suite)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Suite)",
            "AfterTestAction(Suite)",

            "BeforeTestAction(Test)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Test)",
            "AfterTestAction(Test)",

            nameof(TestClassWithTestAction.TestUnderTest),

            "BeforeTestAction(Test)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Test)",
            "AfterTestAction(Test)",

            "BeforeTestAction(Suite)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Suite)",
            "AfterTestAction(Suite)"
        ]));
    }
}
