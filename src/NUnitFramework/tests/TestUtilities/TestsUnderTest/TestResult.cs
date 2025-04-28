// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

internal class TestResult(int runnersErrorCode, string consoleOutput, string[] testLogs, TestRunResult testRunResult)
{
    public string[] Logs { get; } = testLogs;
    public TestRunResult TestRunResult { get; } = testRunResult;
}
