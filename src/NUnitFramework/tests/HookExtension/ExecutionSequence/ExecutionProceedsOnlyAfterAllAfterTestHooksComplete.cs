// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnitLite;

namespace NUnit.Framework.Tests.HookExtension.ExecutionSequence
{
    [Explicit]
    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksComplete
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

        [Test]
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

    internal class ExecutionProceedsOnlyAfterAllAfterTestHooksCompleteTests
    {
        [Test]
        public void TestProceedsAfterAllAfterTestHooksExecute()
        {
            TestLog.Logs.Clear();

            new AutoRun(typeof(ExecutionProceedsOnlyAfterAllAfterTestHooksComplete).Assembly).Execute([
                "--where",
                $"class == {typeof(ExecutionProceedsOnlyAfterAllAfterTestHooksComplete).FullName}"
            ]);
            
            Assert.That(TestLog.Logs, Is.EqualTo([
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksComplete.TestPasses),

                "AfterTestHook_Hook",
                
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksComplete.TearDown),
                nameof(ExecutionProceedsOnlyAfterAllAfterTestHooksComplete.OneTimeTearDown)
            ]));

            TestLog.Logs.Clear();
        }
    }
}
