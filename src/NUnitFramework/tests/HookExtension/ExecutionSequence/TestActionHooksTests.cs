using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence;

public class TestActionLoggingHookExtension : NUnitAttribute, IApplyToContext
{
    public void ApplyToContext(TestExecutionContext context)
    {
        context.HookExtension?.BeforeTestActionBeforeTestHook.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"BeforeTestActionBeforeTestHook({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.AfterTestActionBeforeTestHook.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"AfterTestActionBeforeTestHook({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.BeforeTestActionAfterTestHook.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"BeforeTestActionAfterTestHook({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.AfterTestActionAfterTestHook.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"AfterTestActionAfterTestHook({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
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
            "BeforeTestActionBeforeTestHook(Suite)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Suite)",
            "AfterTestActionBeforeTestHook(Suite)",

            "BeforeTestActionBeforeTestHook(Test)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Test)",
            "AfterTestActionBeforeTestHook(Test)",

            nameof(TestClassWithTestAction.TestUnderTest),

            "BeforeTestActionAfterTestHook(Test)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Test)",
            "AfterTestActionAfterTestHook(Test)",

            "BeforeTestActionAfterTestHook(Suite)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Suite)",
            "AfterTestActionAfterTestHook(Suite)"
        ]));
    }
}
