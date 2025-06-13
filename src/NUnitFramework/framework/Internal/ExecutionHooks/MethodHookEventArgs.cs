// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Eventargs for <see cref="ExecutionHooks"></see> that provides
    /// access to <see cref="TestExecutionContext"></see>.
    /// </summary>
    /// <param name="context">The test execution context.</param>
    public sealed class MethodHookEventArgs(TestExecutionContext context) : EventArgs
    {
        /// <summary>
        /// Gets the test execution context.
        /// </summary>
        public TestExecutionContext Context { get; } = context;
    }
}
