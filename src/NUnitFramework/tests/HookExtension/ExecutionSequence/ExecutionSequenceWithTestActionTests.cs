// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class SomeTestActionAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            TestLog.LogCurrentMethod("BeforeTest_Action");
        }

        public void AfterTest(ITest test)
        {
            TestLog.LogCurrentMethod("AfterTest_Action");
        }

        public ActionTargets Targets { get; }
    }

    [Explicit]
    [SomeTestAction]
    internal class ExecutionSequenceWithTestAction
    {
        private sealed class ActivateAfterTestHooksAttribute : NUnitAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod("AfterTestHook_Hook");
                });
            }
        }

        private sealed class ActivateBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext
        {
            public void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod("BeforeTestHook_Hook");
                });
            }
        }

        [Test]
        [ActivateBeforeTestHooks]
        [ActivateAfterTestHooks]
        public void TestPasses()
        {
            TestLog.LogCurrentMethod();
        }

        [TearDown]
        public void TearDown()
        {
            TestLog.LogCurrentMethod();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.LogCurrentMethod();
        }
    }

    internal class ExecutionSequenceWithTestActionTests
    {
        [Test]
        public void ExecutionSequenceWithTestActionTest()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(ExecutionSequenceWithTestAction).Assembly).Execute([
                "--where",
                $"class == {typeof(ExecutionSequenceWithTestAction).FullName}"
            ]);

            Assert.That(TestLog.Logs, Is.EqualTo([
                "BeforeTest_Action",
                "BeforeTestHook_Hook",
                nameof(ExecutionSequenceWithTestAction.TestPasses),
                "AfterTestHook_Hook",
                nameof(ExecutionSequenceWithTestAction.TearDown),
                "AfterTest_Action",
                nameof(ExecutionSequenceWithTestAction.OneTimeTearDown)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
