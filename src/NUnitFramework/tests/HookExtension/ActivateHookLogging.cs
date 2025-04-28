// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.HookExtensions;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension;

internal class ActivateHookLogging : NUnitAttribute, IApplyToContext
{
    public static LoggerHook LoggingHook = null!;

    public virtual void ApplyToContext(TestExecutionContext context)
    {
        LoggingHook = new LoggerHook();
        context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) => LoggingHook.BeforeTest(sender, eventArgs));
        context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) => LoggingHook.AfterTest(sender, eventArgs));
    }
}

internal class AssemblyLoggingHookExtension : ActivateHookLogging, ITestAction
{
    public override void ApplyToContext(TestExecutionContext context)
    {
        BeforeTestRunHook();
        LoggingHook = null!;
        base.ApplyToContext(context);
    }

    public void BeforeTestRunHook() => TestLog.Log("BeforeTestRunHook");

    public void AfterTestRunHook() => TestLog.Log("AfterTestRunHook");

    public void BeforeTest(ITest test)
    {
    }

    public void AfterTest(ITest test)
    {
        AfterTestRunHook();
    }

    public ActionTargets Targets => ActionTargets.Suite;
}

internal class LoggerHook
{
    public void BeforeTest(object? sender, MethodHookEventArgs eventArgs)
    {
        TestLog.Log($"- BeforeTestCase({eventArgs.Context.CurrentTest.MethodName})");
    }

    public void AfterTest(object? sender, MethodHookEventArgs eventArgs)
    {
        TestLog.Log($"- AfterTestCase({eventArgs.Context.CurrentTest.MethodName})");
    }
}
