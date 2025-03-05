using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence;

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

public class TestActionHooksTests
{
    [LogTestAction]
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
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Suite)",
            $"{nameof(LogTestActionAttribute.BeforeTest)}(Test)",
            nameof(TestClassWithTestAction.TestUnderTest),
            $"{nameof(LogTestActionAttribute.AfterTest)}(Test)",
            $"{nameof(LogTestActionAttribute.AfterTest)}(Suite)"
        ]));
    }
}
