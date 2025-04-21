using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence;

public class TestActionLoggingHookExtension : NUnitAttribute, IApplyToContext
{
    public void ApplyToContext(TestExecutionContext context)
    {
        context.HookExtension?.BeforeTestActionBeforeTest.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"BeforeTestActionBeforeTest({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.AfterTestActionBeforeTest.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"AfterTestActionBeforeTest({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.BeforeTestActionAfterTest.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"BeforeTestActionAfterTest({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
        });
        context.HookExtension?.AfterTestActionAfterTest.AddHandler((sender, eventArgs) =>
        {
            TestLog.LogCurrentMethod($"AfterTestActionAfterTest({(eventArgs.Context.CurrentTest.IsSuite ? "Suite" : "Test")})");
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
            "BeforeTestActionBeforeTest(Suite)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Suite)",
            "AfterTestActionBeforeTest(Suite)",

            "BeforeTestActionBeforeTest(Test)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Test)",
            "AfterTestActionBeforeTest(Test)",

            nameof(TestClassWithTestAction.TestUnderTest),

            "BeforeTestActionAfterTest(Test)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Test)",
            "AfterTestActionAfterTest(Test)",

            "BeforeTestActionAfterTest(Suite)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Suite)",
            "AfterTestActionAfterTest(Suite)"
        ]));
    }
}
