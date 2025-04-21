// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.HookExtensions;

/// <summary>
/// Represents event arguments for type hook methods.
/// </summary>
public class TypeHookEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeHookEventArgs"/> class.
    /// </summary>
    /// <param name="context">The test execution context.</param>
    /// <param name="type">The type information.</param>
    /// <param name="exceptionContext">The exception context that was thrown during the method execution, if any.</param>
    public TypeHookEventArgs(TestExecutionContext context, ITypeInfo type, Exception? exceptionContext = null)
    {
        Context = context;
        Type = type;
        ExceptionContext = exceptionContext;
    }

    /// <summary>
    /// Gets the test execution context.
    /// </summary>
    public TestExecutionContext Context { get; }

    /// <summary>
    /// Gets the type information.
    /// </summary>
    public ITypeInfo Type { get; }

    /// <summary>
    /// Gets the exception context that was thrown during the method execution, if any.
    /// </summary>
    public Exception? ExceptionContext { get; }
}
