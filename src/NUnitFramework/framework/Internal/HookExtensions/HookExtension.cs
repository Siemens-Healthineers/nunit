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
        BeforeAnySetUps = new AsyncEvent<MethodHookEventArgs>();
        AfterAnySetUps = new AsyncEvent<MethodHookEventArgs>();
        BeforeTest = new AsyncEvent<TestHookEventArgs>();
        AfterTest = new AsyncEvent<TestHookEventArgs>();
        BeforeAnyTearDowns = new AsyncEvent<MethodHookEventArgs>();
        AfterAnyTearDowns = new AsyncEvent<MethodHookEventArgs>();

        BeforeTestAction = new AsyncEvent<MethodHookEventArgs>();
        AfterTestAction = new AsyncEvent<MethodHookEventArgs>();
    }

    /// <summary>
    /// Gets or sets the hook event that is triggered before any setup methods are executed.
    /// </summary>
    public AsyncEvent<MethodHookEventArgs> BeforeAnySetUps { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after any setup methods are executed.
    /// </summary>
    public AsyncEvent<MethodHookEventArgs> AfterAnySetUps { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before a test method is executed.
    /// </summary>
    public AsyncEvent<TestHookEventArgs> BeforeTest { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after a test method is executed.
    /// </summary>
    public AsyncEvent<TestHookEventArgs> AfterTest { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before any teardown methods are executed.
    /// </summary>
    public AsyncEvent<MethodHookEventArgs> BeforeAnyTearDowns { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after any teardown methods are executed.
    /// </summary>
    public AsyncEvent<MethodHookEventArgs> AfterAnyTearDowns { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before a test action is executed.
    /// </summary>
    public AsyncEvent<MethodHookEventArgs> BeforeTestAction { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after a test action is executed.
    /// </summary>
    public AsyncEvent<MethodHookEventArgs> AfterTestAction { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
    /// </summary>
    /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
    public HookExtension(HookExtension other) : this()
    {
        other.BeforeAnySetUps.GetHandlers().ToList().ForEach(d => BeforeAnySetUps.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterAnySetUps.GetHandlers().ToList().ForEach(d => AfterAnySetUps.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.BeforeTest.GetHandlers().ToList().ForEach(d => BeforeTest.AddHandler((EventHandler<TestHookEventArgs>)d));
        other.AfterTest.GetHandlers().ToList().ForEach(d => AfterTest.AddHandler((EventHandler<TestHookEventArgs>)d));
        other.BeforeAnyTearDowns.GetHandlers().ToList().ForEach(d => BeforeAnyTearDowns.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterAnyTearDowns.GetHandlers().ToList().ForEach(d => AfterAnyTearDowns.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.BeforeTestAction.GetHandlers().ToList().ForEach(d => BeforeTestAction.AddHandler((EventHandler<MethodHookEventArgs>)d));
        other.AfterTestAction.GetHandlers().ToList().ForEach(d => AfterTestAction.AddHandler((EventHandler<MethodHookEventArgs>)d));

        other.BeforeAnySetUps.GetAsyncHandlers().ToList().ForEach(d => BeforeAnySetUps.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterAnySetUps.GetAsyncHandlers().ToList().ForEach(d => AfterAnySetUps.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.BeforeTest.GetAsyncHandlers().ToList().ForEach(d => BeforeTest.AddAsyncHandler((AsyncEventHandler<TestHookEventArgs>)d));
        other.AfterTest.GetAsyncHandlers().ToList().ForEach(d => AfterTest.AddAsyncHandler((AsyncEventHandler<TestHookEventArgs>)d));
        other.BeforeAnyTearDowns.GetAsyncHandlers().ToList().ForEach(d => BeforeAnyTearDowns.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterAnyTearDowns.GetAsyncHandlers().ToList().ForEach(d => AfterAnyTearDowns.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.BeforeTestAction.GetAsyncHandlers().ToList().ForEach(d => BeforeTestAction.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
        other.AfterTestAction.GetAsyncHandlers().ToList().ForEach(d => AfterTestAction.AddAsyncHandler((AsyncEventHandler<MethodHookEventArgs>)d));
    }

    internal void OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo method)
    {
        BeforeAnySetUps.Invoke(this, new MethodHookEventArgs(context, method));
    }

    internal void OnAfterAnySetUps(TestExecutionContext context, IMethodInfo method, Exception? exceptionContext = null)
    {
        AfterAnySetUps.Invoke(this, new MethodHookEventArgs(context, method, exceptionContext));
    }

    internal void OnBeforeTest(TestExecutionContext context)
    {
        BeforeTest.Invoke(this, new TestHookEventArgs(context));
    }

    internal void OnAfterTest(TestExecutionContext context, Exception? exceptionContext = null)
    {
        AfterTest.Invoke(this, new TestHookEventArgs(context, exceptionContext));
    }

    internal void OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo method)
    {
        BeforeAnyTearDowns.Invoke(this, new MethodHookEventArgs(context, method));
    }

    internal void OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo method, Exception? exceptionContext = null)
    {
        AfterAnyTearDowns.Invoke(this, new MethodHookEventArgs(context, method, exceptionContext));
    }

    internal void OnBeforeTestAction(TestExecutionContext context, IMethodInfo testActionMethodInfo)
    {
        BeforeTestAction.Invoke(this, new MethodHookEventArgs(context, testActionMethodInfo));
    }

    internal void OnAfterTestAction(TestExecutionContext context, IMethodInfo testActionMethodInfo, Exception? exceptionContext = null)
    {
        AfterTestAction.Invoke(this, new MethodHookEventArgs(context, testActionMethodInfo, exceptionContext));
    }
}

/// <summary/>
public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
