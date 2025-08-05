// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    public class OneTestWithLoggingHooksAndOneWithout
    {
        [Explicit($"This test should only be run as part of the {nameof(CheckLoggingTest)} test")]
        public class TestUnderTest
        {
            [Test, ActivateTestHook, Order(1)]
            public void TestWithHookLogging()
            {
                TestLog.LogCurrentMethod();
            }

            [Test, Order(2)]
            public void TestWithoutHookLogging()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void CheckLoggingTest()
        {
            // Capture current context logs reference
            var currentTestLogs = TestLog.Logs;
            currentTestLogs.Clear();

            var workItem = TestBuilder.CreateWorkItem(typeof(TestUnderTest), TestFilter.Explicit);
            workItem.Execute();

            Assert.Multiple(() =>
            {
                Assert.That(currentTestLogs, Is.Not.Empty);
                Assert.That(currentTestLogs, Is.EqualTo([
                    $"{HookIdentifiers.BeforeTestHook}({nameof(TestUnderTest.TestWithHookLogging)})",
                    nameof(TestUnderTest.TestWithHookLogging),
                    $"{HookIdentifiers.AfterTestHook}({nameof(TestUnderTest.TestWithHookLogging)})",
                    nameof(TestUnderTest.TestWithoutHookLogging)
                ]));
            });
        }
    }
}
