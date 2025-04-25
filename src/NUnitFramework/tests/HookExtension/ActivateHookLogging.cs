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
        context?.HookExtension?.BeforeAnySetUpsHook.AddHandler((sender, eventArgs) => LoggingHook.BeforeAnySetUps(sender, eventArgs));
        context?.HookExtension?.AfterAnySetUpsHook.AddHandler((sender, eventArgs) => LoggingHook.AfterAnySetUps(sender, eventArgs));
        context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) => LoggingHook.BeforeTest(sender, eventArgs));
        context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) => LoggingHook.AfterTest(sender, eventArgs));
        context?.HookExtension?.BeforeAnyTearDownsHook.AddHandler((sender, eventArgs) => LoggingHook.BeforeAnyTearDowns(sender, eventArgs));
        context?.HookExtension?.AfterAnyTearDownsHook.AddHandler((sender, eventArgs) => LoggingHook.AfterAnyTearDowns(sender, eventArgs));
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
    public void BeforeOneTimeSetUp(string methodName) => TestLog.Log($"- BeforeOneTimeSetUp({methodName})");
    public void AfterOneTimeSetUp(string methodName) => TestLog.Log($"- AfterOneTimeSetUp({methodName})");
    public void BeforeSetUp(string methodName) => TestLog.Log($"- BeforeSetUp({methodName})");
    public void AfterSetUp(string methodName) => TestLog.Log($"- AfterSetUp({methodName})");
    public void BeforeTearDown(string methodName) => TestLog.Log($"- BeforeTearDown({methodName})");
    public void AfterTearDown(string methodName) => TestLog.Log($"- AfterTearDown({methodName})");
    public void BeforeOneTimeTearDown(string methodName) => TestLog.Log($"- BeforeOneTimeTearDown({methodName})");
    public void AfterOneTimeTearDown(string methodName) => TestLog.Log($"- AfterOneTimeTearDown({methodName})");

    public void BeforeAnySetUps(object? sender, MethodHookEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            BeforeOneTimeSetUp(eventArgs.HookedMethod.Name);
        }
        else
        {
            BeforeSetUp(eventArgs.HookedMethod.Name);
        }
    }

    public void AfterAnySetUps(object? sender, MethodHookEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            AfterOneTimeSetUp(eventArgs.HookedMethod.Name);
        }
        else
        {
            AfterSetUp(eventArgs.HookedMethod.Name);
        }
    }

    public void BeforeAnyTearDowns(object? sender, MethodHookEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            BeforeOneTimeTearDown(eventArgs.HookedMethod.Name);
        }
        else
        {
            BeforeTearDown(eventArgs.HookedMethod.Name);
        }
    }

    public void AfterAnyTearDowns(object? sender, MethodHookEventArgs eventArgs)
    {
        if (eventArgs.Context.CurrentTest.IsSuite)
        {
            AfterOneTimeTearDown(eventArgs.HookedMethod.Name);
        }
        else
        {
            AfterTearDown(eventArgs.HookedMethod.Name);
        }
    }

    public void BeforeTest(object? sender, MethodHookEventArgs eventArgs)
    {
        TestLog.Log($"- BeforeTestCase({eventArgs.Context.CurrentTest.MethodName})");
    }

    public void AfterTest(object? sender, MethodHookEventArgs eventArgs)
    {
        TestLog.Log($"- AfterTestCase({eventArgs.Context.CurrentTest.MethodName})");
    }
}
