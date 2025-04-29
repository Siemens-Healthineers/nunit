// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    [Explicit]
    internal class ExecutionSequenceWithBeforeAndAfterTestHooks
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

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestLog.LogCurrentMethod();
        }

        [SetUp]
        public void Setup()
        {
            TestLog.LogCurrentMethod();
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

    internal class ExecutionSequenceWithBeforeAndAfterTestHooksTests
    {
        [Test]
        public void ExecutionSequenceWithBeforeAndAfterTestHooksTest()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(ExecutionSequenceWithBeforeAndAfterTestHooks).Assembly).Execute([
                "--where",
                $"class == {typeof(ExecutionSequenceWithBeforeAndAfterTestHooks).FullName}"
            ]);

            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(ExecutionSequenceWithBeforeAndAfterTestHooks.OneTimeSetUp),
                nameof(ExecutionSequenceWithBeforeAndAfterTestHooks.Setup),
                
                "BeforeTestHook_Hook",
                nameof(ExecutionSequenceWithBeforeAndAfterTestHooks.TestPasses),
                $"AfterTestHook_Hook",

                nameof(ExecutionSequenceWithBeforeAndAfterTestHooks.TearDown),
                nameof(ExecutionSequenceWithBeforeAndAfterTestHooks.OneTimeTearDown)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
