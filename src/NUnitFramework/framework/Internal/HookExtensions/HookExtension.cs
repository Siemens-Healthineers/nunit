// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions;

/// <summary>
/// Hook Extension interface to run custom code synchronously before or after any test activity.
/// </summary>
public class HookExtension
{
    /// <summary>
    /// Default ctor of <see cref="HookExtension"/> class.
    /// </summary>
    public HookExtension()
    {
        BeforeAnySetUpsHook = new TestHook<MethodHookEventArgs>();
        AfterAnySetUpsHook = new TestHook<MethodHookEventArgs>();
        BeforeTestHook = new TestHook<MethodHookEventArgs>();
        AfterTestHook = new TestHook<MethodHookEventArgs>();
        BeforeAnyTearDownsHook = new TestHook<MethodHookEventArgs>();
        AfterAnyTearDownsHook = new TestHook<MethodHookEventArgs>();

        BeforeTestActionBeforeTestHook = new TestHook<MethodHookEventArgs>();
        AfterTestActionBeforeTestHook = new TestHook<MethodHookEventArgs>();
        BeforeTestActionAfterTestHook = new TestHook<MethodHookEventArgs>();
        AfterTestActionAfterTestHook = new TestHook<MethodHookEventArgs>();
    }

    /// <summary>
    /// Gets or sets the hook event that is triggered before any setup methods are executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> BeforeAnySetUpsHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after any setup methods are executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> AfterAnySetUpsHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before a test method is executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> BeforeTestHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after a test method is executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> AfterTestHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before any teardown methods are executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> BeforeAnyTearDownsHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after any teardown methods are executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> AfterAnyTearDownsHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before a BeforeTestHook test action is executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> BeforeTestActionBeforeTestHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after a BeforeTestHook test action is executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> AfterTestActionBeforeTestHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before a AfterTestHook test action is executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> BeforeTestActionAfterTestHook { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after a AfterTestHook test action is executed.
    /// </summary>
    public TestHook<MethodHookEventArgs> AfterTestActionAfterTestHook { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
    /// </summary>
    /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
    public HookExtension(HookExtension other) : this()
    {
        other.BeforeAnySetUpsHook.GetHandlers().ToList().ForEach(d => BeforeAnySetUpsHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterAnySetUpsHook.GetHandlers().ToList().ForEach(d => AfterAnySetUpsHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.BeforeTestHook.GetHandlers().ToList().ForEach(d => BeforeTestHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterTestHook.GetHandlers().ToList().ForEach(d => AfterTestHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.BeforeAnyTearDownsHook.GetHandlers().ToList().ForEach(d => BeforeAnyTearDownsHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterAnyTearDownsHook.GetHandlers().ToList().ForEach(d => AfterAnyTearDownsHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.BeforeTestActionBeforeTestHook.GetHandlers().ToList().ForEach(d => BeforeTestActionBeforeTestHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterTestActionBeforeTestHook.GetHandlers().ToList().ForEach(d => AfterTestActionBeforeTestHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.BeforeTestActionAfterTestHook.GetHandlers().ToList().ForEach(d => BeforeTestActionAfterTestHook.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterTestActionAfterTestHook.GetHandlers().ToList().ForEach(d => AfterTestActionAfterTestHook.AddHandler((EventHandler<MethodHookEventArgs>)d));

        other.BeforeAnySetUpsHook.GetAsyncHandlers().ToList().ForEach(d => BeforeAnySetUpsHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterAnySetUpsHook.GetAsyncHandlers().ToList().ForEach(d => AfterAnySetUpsHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.BeforeTestHook.GetAsyncHandlers().ToList().ForEach(d => BeforeTestHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterTestHook.GetAsyncHandlers().ToList().ForEach(d => AfterTestHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.BeforeAnyTearDownsHook.GetAsyncHandlers().ToList().ForEach(d => BeforeAnyTearDownsHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterAnyTearDownsHook.GetAsyncHandlers().ToList().ForEach(d => AfterAnyTearDownsHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.BeforeTestActionBeforeTestHook.GetAsyncHandlers().ToList().ForEach(d => BeforeTestActionBeforeTestHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterTestActionBeforeTestHook.GetAsyncHandlers().ToList().ForEach(d => AfterTestActionBeforeTestHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.BeforeTestActionAfterTestHook.GetAsyncHandlers().ToList().ForEach(d => BeforeTestActionAfterTestHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterTestActionAfterTestHook.GetAsyncHandlers().ToList().ForEach(d => AfterTestActionAfterTestHook.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
    }

    internal void OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo hookedMethod)
    {
        BeforeAnySetUpsHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod));
    }

    internal void OnAfterAnySetUps(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
    {
        AfterAnySetUpsHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod, exceptionContext));
    }

    internal void OnBeforeTest(TestExecutionContext context, IMethodInfo hookedMethod)
    {
        BeforeTestHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod));
    }

    internal void OnAfterTest(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
    {
        AfterTestHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod, exceptionContext));
    }

    internal void OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo hookedMethod)
    {
        BeforeAnyTearDownsHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod));
    }

    internal void OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
    {
        AfterAnyTearDownsHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod, exceptionContext));
    }

    internal void OnBeforeTestActionBeforeTest(TestExecutionContext context, IMethodInfo hookedMethod)
    {
        BeforeTestActionBeforeTestHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod));
    }

    internal void OnAfterTestActionBeforeTest(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
    {
        AfterTestActionBeforeTestHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod, exceptionContext));
    }

    internal void OnBeforeTestActionAfterTest(TestExecutionContext context, IMethodInfo hookedMethod)
    {
        BeforeTestActionAfterTestHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod));
    }

    internal void OnAfterTestActionAfterTest(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
    {
        AfterTestActionAfterTestHook.InvokeHandlers(this, new MethodHookEventArgs(context, hookedMethod, exceptionContext));
    }
}

/// <summary/>
public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
