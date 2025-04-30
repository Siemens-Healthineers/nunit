// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions;

/// <summary>
/// Represents event arguments for test hook methods.
/// </summary>
public class MethodHookEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodHookEventArgs"/> class.
    /// </summary>
    /// <param name="context">The test execution context.</param>
    /// <param name="hookedMethod">The hookedMethod information.</param>
    /// <param name="exceptionContext">The exception context that was thrown during the hookedMethod execution, if any.</param>
    public MethodHookEventArgs(TestExecutionContext context, IMethodInfo hookedMethod, Exception? exceptionContext = null)
    {
        Context = context;
        HookedMethod = hookedMethod;
        ExceptionContext = exceptionContext;
    }

    /// <summary>
    /// Gets the test execution context.
    /// </summary>
    public TestExecutionContext Context { get; }

    /// <summary>
    /// Gets the method information of the hooked method.
    /// </summary>
    public IMethodInfo HookedMethod { get; }

    /// <summary>
    /// Gets the exception context that was thrown during the method execution, if any.
    /// </summary>
    public Exception? ExceptionContext { get; }
}
