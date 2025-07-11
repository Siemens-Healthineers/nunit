// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

public static class TestLog
{
    private static readonly AsyncLocal<List<string>> Intlogs = new AsyncLocal<List<string>>();
    private static readonly object LogLock = new object();

    // Each aync context gets its own instance of Logs
    public static List<string> Logs
    {
        get
        {
            lock (LogLock)
            {
                if (Intlogs.Value is null)
                {
                    Intlogs.Value = new List<string>();
                }
                return Intlogs.Value;
            }
        }
    }

    public static void Log(string infoToLog)
    {
        Logs.Add(infoToLog);
    }

    public static void LogCurrentMethod([CallerMemberName] string callerMethodName = "")
    {
        Log(callerMethodName);
    }

    public static void LogCurrentMethodWithContextInfo(string contextInfo, [CallerMemberName] string callerMethodName = "")
    {
        Log($"{callerMethodName}({contextInfo})");
    }
}
