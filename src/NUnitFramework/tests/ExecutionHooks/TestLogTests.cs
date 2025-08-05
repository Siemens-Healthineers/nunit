//Copyright(c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.ExecutionHooks
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class TestLogTests
    {
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            TestLog.Clear();
        }

        [Test]
        public void ParallelTestsIsolatedLogsTest1()
        {
            TestLog.LogMessage("Message1");
            Thread.Sleep(100);

            var testLogs = TestLog.FetchLogsForTest(TestExecutionContext.CurrentContext.CurrentTest);
            Assert.That(testLogs.Contains("Message1"));
        }

        [Test]
        public void ParallelTestsIsolatedLogsTest2()
        {
            TestLog.LogMessage("Message2");
            Thread.Sleep(200);

            var testLogs = TestLog.FetchLogsForTest(TestExecutionContext.CurrentContext.CurrentTest);
            Assert.That(testLogs.Contains("Message2"));
        }

        [Test]
        public void ParallelTestsIsolatedLogsTest3()
        {
            TestLog.LogMessage("Message3");
            Thread.Sleep(300);

            var testLogs = TestLog.FetchLogsForTest(TestExecutionContext.CurrentContext.CurrentTest);
            Assert.That(testLogs.Contains("Message3"));
        }

        [Test]
        public void TestUnderTest()
        {
            var workItem = TestBuilder.CreateWorkItem(typeof(ExplicitTestUnderTest), TestFilter.Explicit);
            workItem.Execute();
            var testLogs = TestLog.FetchLogsForTest(workItem.Test);

            var expectedMessages = new List<string>()
            {
                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestOneTimeSetUp),

                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestSetUp),
                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestEmptyTest),
                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestTearDown),

                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestSetUp),
                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestEmptyTest),
                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestTearDown),

                nameof(ExplicitTestUnderTest.ExplicitTestUnderTestOneTimeTearDown)
            };

            Assert.That(expectedMessages.All(expectedMessage => testLogs.Contains(expectedMessage)));
        }

        [Explicit($"This test should only be run as part of the TestUnderTest test")]
        private class ExplicitTestUnderTest
        {
            [OneTimeSetUp]
            public void ExplicitTestUnderTestOneTimeSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [OneTimeTearDown]
            public void ExplicitTestUnderTestOneTimeTearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [SetUp]
            public void ExplicitTestUnderTestSetUp()
            {
                TestLog.LogCurrentMethod();
            }

            [TearDown]
            public void ExplicitTestUnderTestTearDown()
            {
                TestLog.LogCurrentMethod();
            }

            [Test]
            [Repeat(2)]
            public void ExplicitTestUnderTestEmptyTest()
            {
                TestLog.LogCurrentMethod();
            }
        }
    }
}
