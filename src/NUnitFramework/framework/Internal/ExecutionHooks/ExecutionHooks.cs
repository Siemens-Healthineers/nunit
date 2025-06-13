// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Provides hooks for executing custom code before and after test methods.
    /// </summary>
    public sealed class ExecutionHooks
    {
        internal ExecutionHooks()
        {
        }

        internal TestHook<MethodHookEventArgs> BeforeTest { get; } = new();
        internal TestHook<MethodHookEventArgs> AfterTest { get; } = new();

        internal TestHook<MethodHookEventArgs> BeforeTestActionBeforeTest { get; set; } = new();
        internal TestHook<MethodHookEventArgs> AfterTestActionBeforeTest { get; set; } = new();
        internal TestHook<MethodHookEventArgs> BeforeTestActionAfterTest { get; set; } = new();
        internal TestHook<MethodHookEventArgs> AfterTestActionAfterTest { get; set; } = new();

        /// <summary>
        /// Adds a hook handler to be invoked before the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the before-test hook.</param>
        public void AddBeforeTestHandler(EventHandler<MethodHookEventArgs> hookHandler)
        {
            BeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the test method is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the after-test hook.</param>
        public void AddAfterTestHandler(EventHandler<MethodHookEventArgs> hookHandler)
        {
            AfterTest.AddHandler(hookHandler);
        }

        internal ExecutionHooks(ExecutionHooks other)
        {
            BeforeTest = new TestHook<MethodHookEventArgs>(other.BeforeTest);
            AfterTest = new TestHook<MethodHookEventArgs>(other.AfterTest);

            BeforeTestActionBeforeTest = new TestHook<MethodHookEventArgs>(other.BeforeTestActionBeforeTest);
            AfterTestActionBeforeTest = new TestHook<MethodHookEventArgs>(other.AfterTestActionBeforeTest);
            BeforeTestActionAfterTest = new TestHook<MethodHookEventArgs>(other.BeforeTestActionAfterTest);
            AfterTestActionAfterTest = new TestHook<MethodHookEventArgs>(other.AfterTestActionAfterTest);
        }

        /// <summary>
        /// Adds a hook handler to be invoked before the BeforeTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the before-test hook.</param>
        public void AddBeforeTestActionBeforeTestHandler(EventHandler<MethodHookEventArgs> hookHandler)
        {
            BeforeTestActionBeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the BeforeTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the after-test hook.</param>
        public void AddAfterTestActionBeforeTestHandler(EventHandler<MethodHookEventArgs> hookHandler)
        {
            AfterTestActionBeforeTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked before the AfterTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the before-test hook.</param>
        public void AddBeforeTestActionAfterTestHandler(EventHandler<MethodHookEventArgs> hookHandler)
        {
            BeforeTestActionAfterTest.AddHandler(hookHandler);
        }

        /// <summary>
        /// Adds a hook handler to be invoked after the AfterTest(ITest test) method of an <see cref="ITestAction"></see> is executed.
        /// </summary>
        /// <param name="hookHandler">The event handler to attach to the after-test hook.</param>
        public void AddAfterTestActionAfterTestHandler(EventHandler<MethodHookEventArgs> hookHandler)
        {
            AfterTestActionAfterTest.AddHandler(hookHandler);
        }

        internal void OnBeforeTest(TestExecutionContext context)
        {
            BeforeTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnAfterTest(TestExecutionContext context)
        {
            AfterTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnBeforeTestActionBeforeTest(TestExecutionContext context)
        {
            BeforeTestActionBeforeTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnAfterTestActionBeforeTest(TestExecutionContext context)
        {
            AfterTestActionBeforeTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnBeforeTestActionAfterTest(TestExecutionContext context)
        {
            BeforeTestActionAfterTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }

        internal void OnAfterTestActionAfterTest(TestExecutionContext context)
        {
            AfterTestActionAfterTest.InvokeHandlers(this, new MethodHookEventArgs(context));
        }
    }
}
