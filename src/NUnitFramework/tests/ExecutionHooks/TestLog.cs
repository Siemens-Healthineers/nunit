// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks
{
    /// <summary>
    /// Class to log messages during test execution per Test case.
    /// It handles also Test-under-Test scenarios, where the test is executed in the context of a fixture (suite).
    /// As OneTimeSetUp and OneTimeTearDown are executed in the context of the fixture (suite),
    /// and their TestContext.CurrentContext.Test.ID is different from the IDs of the individual test methods, the logs are
    /// accumulated based on parent test ID.
    /// </summary>
    internal class TestLog
    {
        private class LogEntry
        {
            public string Message { get; init; } = string.Empty;
            public int Sequence { get; init; }
        }

        private static readonly Dictionary<string, List<LogEntry>> _localLogs = new();
        private static readonly Dictionary<string, int> _logSequence = new();
        private static readonly object LogLock = new();

        public static void LogCurrentMethod([CallerMemberName] string callerMethodName = "")
        {
            AddLog(callerMethodName);
        }

        public static void LogCurrentMethodWithContextInfo(string contextInfo,
            [CallerMemberName] string callerMethodName = "")
        {
            AddLog($"{callerMethodName}({contextInfo})");
        }

        public static void LogMessage(string message)
        {
            AddLog(message);
        }

        public static void Clear()
        {
            var parentTestId = GetRootParentId(TestExecutionContext.CurrentContext.CurrentTest);

            lock (LogLock)
            {
                _localLogs.Remove(parentTestId);
                _logSequence.Remove(parentTestId);
            }
        }

        public static List<string> FetchLogsForTest(ITest test)
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }
            
            var rootParentTestId = GetRootParentId(test);

            lock (LogLock)
            {
                if (_localLogs.TryGetValue(rootParentTestId, out var logs))
                {
                    return logs.OrderBy(entry => entry.Sequence)
                               .Select(entry => entry.Message)
                               .ToList();
                }
            }

            return [];
        }

        private static string GetRootParentId(ITest currentTest)
        {
            while (currentTest.Parent != null)
            {
                currentTest = currentTest.Parent;
            }
            return currentTest.Id;
        }
        
        private static void AddLog(string message)
        {
            var parentTestId = GetRootParentId(TestExecutionContext.CurrentContext.CurrentTest);
            if (string.IsNullOrEmpty(parentTestId))
            {
                throw new InvalidOperationException("Current test does not have a valid parent test ID.");
            }

            lock (LogLock)
            {
                if (!_localLogs.ContainsKey(parentTestId))
                {
                    _localLogs[parentTestId] = [];
                    _logSequence[parentTestId] = 0;
                }

                _localLogs[parentTestId].Add(new LogEntry
                {
                    Message = message,
                    Sequence = ++_logSequence[parentTestId]
                });
            }
        }
    }
}
