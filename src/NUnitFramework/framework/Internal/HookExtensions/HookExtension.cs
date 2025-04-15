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
        BeforeAnySetUps = new AsyncEvent<TestHookIMethodEventArgs>();
        AfterAnySetUps = new AsyncEvent<TestHookIMethodEventArgs>();
        BeforeTest = new AsyncEvent<TestHookTestMethodEventArgs>();
        AfterTest = new AsyncEvent<TestHookTestMethodEventArgs>();
        BeforeAnyTearDowns = new AsyncEvent<TestHookIMethodEventArgs>();
        AfterAnyTearDowns = new AsyncEvent<TestHookIMethodEventArgs>();
    }

    /// <summary>
    /// Gets or sets the hook event that is triggered before any setup methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> BeforeAnySetUps { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after any setup methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> AfterAnySetUps { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before a test method is executed.
    /// </summary>
    public AsyncEvent<TestHookTestMethodEventArgs> BeforeTest { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after a test method is executed.
    /// </summary>
    public AsyncEvent<TestHookTestMethodEventArgs> AfterTest { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered before any teardown methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> BeforeAnyTearDowns { get; set; }

    /// <summary>
    /// Gets or sets the hook event that is triggered after any teardown methods are executed.
    /// </summary>
    public AsyncEvent<TestHookIMethodEventArgs> AfterAnyTearDowns { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HookExtension"/> class by copying hooks from another instance.
    /// </summary>
    /// <param name="other">The instance of <see cref="HookExtension"/> to copy hooks from.</param>
    public HookExtension(HookExtension other) : this()
    {
        other.BeforeAnySetUps.GetHandlers().ToList().ForEach(d => BeforeAnySetUps.AddHandler((EventHandler<TestHookIMethodEventArgs>)d));
        other.AfterAnySetUps.GetHandlers().ToList().ForEach(d => AfterAnySetUps.AddHandler((EventHandler<TestHookIMethodEventArgs>)d));
        other.BeforeTest.GetHandlers().ToList().ForEach(d => BeforeTest.AddHandler((EventHandler<TestHookTestMethodEventArgs>)d));
        other.AfterTest.GetHandlers().ToList().ForEach(d => AfterTest.AddHandler((EventHandler<TestHookTestMethodEventArgs>)d));
        other.BeforeAnyTearDowns.GetHandlers().ToList().ForEach(d => BeforeAnyTearDowns.AddHandler((EventHandler<TestHookIMethodEventArgs>)d));
        other.AfterAnyTearDowns.GetHandlers().ToList().ForEach(d => AfterAnyTearDowns.AddHandler((EventHandler<TestHookIMethodEventArgs>)d));

        other.BeforeAnySetUps.GetAsyncHandlers().ToList().ForEach(d => BeforeAnySetUps.AddAsyncHandler((AsyncEventHandler<TestHookIMethodEventArgs>)d));
        other.AfterAnySetUps.GetAsyncHandlers().ToList().ForEach(d => AfterAnySetUps.AddAsyncHandler((AsyncEventHandler<TestHookIMethodEventArgs>)d));
        other.BeforeTest.GetAsyncHandlers().ToList().ForEach(d => BeforeTest.AddAsyncHandler((AsyncEventHandler<TestHookTestMethodEventArgs>)d));
        other.AfterTest.GetAsyncHandlers().ToList().ForEach(d => AfterTest.AddAsyncHandler((AsyncEventHandler<TestHookTestMethodEventArgs>)d));
        other.BeforeAnyTearDowns.GetAsyncHandlers().ToList().ForEach(d => BeforeAnyTearDowns.AddAsyncHandler((AsyncEventHandler<TestHookIMethodEventArgs>)d));
        other.AfterAnyTearDowns.GetAsyncHandlers().ToList().ForEach(d => AfterAnyTearDowns.AddAsyncHandler((AsyncEventHandler<TestHookIMethodEventArgs>)d));
    }

    internal void OnBeforeAnySetUps(TestExecutionContext context, IMethodInfo method)
    {
        await BeforeAnySetUps.Invoke(this, new TestHookIMethodEventArgs(context, method));
    }

    internal void OnAfterAnySetUps(TestExecutionContext context, IMethodInfo method, Exception? exceptionContext = null)
    {
        await AfterAnySetUps.Invoke(this, new TestHookIMethodEventArgs(context, method, exceptionContext));
    }

    internal void OnBeforeTest(TestExecutionContext context)
    {
        await BeforeTest.Invoke(this, new TestHookTestMethodEventArgs(context));
    }

    internal void OnAfterTest(TestExecutionContext context, Exception? exceptionContext = null)
    {
        await AfterTest.Invoke(this, new TestHookTestMethodEventArgs(context, exceptionContext));
    }

    internal void OnBeforeAnyTearDowns(TestExecutionContext context, IMethodInfo method)
    {
        await BeforeAnyTearDowns.Invoke(this, new TestHookIMethodEventArgs(context, method));
    }

    internal void OnAfterAnyTearDowns(TestExecutionContext context, IMethodInfo method, Exception? exceptionContext = null)
    {
        await AfterAnyTearDowns.Invoke(this, new TestHookIMethodEventArgs(context, method, exceptionContext));
    }
}

/// <summary/>
public delegate Task AsyncEventHandler<TEventArgs>(object? sender, TEventArgs e);
