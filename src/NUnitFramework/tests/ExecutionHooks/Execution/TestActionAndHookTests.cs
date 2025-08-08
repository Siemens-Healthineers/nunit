// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Tests.ExecutionHooks.Common;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class TestActionAndHookTests
    {
        [Explicit($"This test should only be run as part of the {nameof(ExecutionProceedsAfterTheAfterTestHookCompletes2)} test")]
        [SimpleTestAction]
        public class TestWithTestHooksAndClassTestActionAttribute
        {
            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void SetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void TearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            [ActivateBeforeTestHook]
            [ActivateAfterTestHook]
            public void EmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }

        [Test]
        public void ExecutionProceedsAfterTheAfterTestHookCompletes2()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(TestWithTestHooksAndClassTestActionAttribute),
                TestFilter.Explicit);
            workItem.Execute();
            var currentTestLogs = TestLog.Logs(workItem.Test);

            Assert.That(currentTestLogs, Is.Not.Empty);
            Assert.That(currentTestLogs, Is.EqualTo([
                nameof(TestWithTestHooksAndClassTestActionAttribute.OneTimeSetUp),
                SimpleTestActionAttribute.LogStringForBeforeTest,
                nameof(TestWithTestHooksAndClassTestActionAttribute.SetUp),
                nameof(ActivateBeforeTestHookAttribute),
                nameof(TestWithTestHooksAndClassTestActionAttribute.EmptyTest),
                nameof(ActivateAfterTestHookAttribute),
                nameof(TestWithTestHooksAndClassTestActionAttribute.TearDown),
                SimpleTestActionAttribute.LogStringForAfterTest,
                nameof(TestWithTestHooksAndClassTestActionAttribute.OneTimeTearDown)
            ]));
        }
    }
}
