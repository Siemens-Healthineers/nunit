// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    [Explicit]
    internal class ExecutionProceedsOnlyAfterAllBeforeTestHooksComplete
    {
        internal class ActivateBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod("BeforeTestHook_Hook");
                });
            }
        }

        internal class ActivateLongRunningBeforeTestHooksAttribute : NUnitAttribute, IApplyToContext
        {
            public virtual void ApplyToContext(TestExecutionContext context)
            {
                context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) =>
                {
                    TestLog.LogCurrentMethod("BeforeTestHook_Hook");
                    Thread.Sleep(5000);
                });
            }
        }

        [Test]
        [ActivateBeforeTestHooks]
        [ActivateLongRunningBeforeTestHooks]
        public void TestPasses()
        {
            TestLog.LogCurrentMethod();

        }
    }

    internal class ExecutionProceedsOnlyAfterAllBeforeTestHooksCompleteTests
    {
        [Test]
        public void TestProceedsOnlyAfterAllBeforeTestHooksCompleteTest()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(ExecutionProceedsOnlyAfterAllBeforeTestHooksComplete).Assembly).Execute([
                "--where",
                $"class == {typeof(ExecutionProceedsOnlyAfterAllBeforeTestHooksComplete).FullName}"
            ]);

            var beforeTestHook = "BeforeTestHook_Hook";

            Assert.That(TestLog.Logs, Is.EqualTo([
                beforeTestHook,
                beforeTestHook,
                nameof(ExecutionProceedsOnlyAfterAllBeforeTestHooksComplete.TestPasses)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
