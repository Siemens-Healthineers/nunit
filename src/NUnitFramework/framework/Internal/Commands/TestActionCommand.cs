// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TestActionCommand handles a single ITestAction applied
    /// to a test. It runs the BeforeTest method, then runs the
    /// test and finally runs the AfterTest method.
    /// </summary>
    public class TestActionCommand : BeforeAndAfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="action">The TestAction with which to wrap the inner command.</param>
        public TestActionCommand(TestCommand innerCommand, ITestAction action)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "TestActionCommand may only apply to a TestMethod", nameof(innerCommand));
            Guard.ArgumentNotNull(action, nameof(action));

            BeforeTest = context =>
            {
                try
                {
                    context.HookExtension?.OnBeforeTestActionBeforeTest(context, new TypeWrapper(action.GetType()));

                    action.BeforeTest(Test);
                }
                catch (Exception exception)
                {
                    // H-TODO: add tests for exception handling
                    context.HookExtension?.OnAfterTestActionBeforeTest(context, new TypeWrapper(action.GetType()), exception);
                    throw;
                }
                context.HookExtension?.OnAfterTestActionBeforeTest(context, new TypeWrapper(action.GetType()));
            };

            AfterTest = context =>
            {
                try
                {
                    context.HookExtension?.OnBeforeTestActionAfterTest(context, new TypeWrapper(action.GetType()));

                    action.AfterTest(Test);
                }
                catch (Exception exception)
                {
                    // H-TODO: add tests for exception handling
                    context.HookExtension?.OnAfterTestActionAfterTest(context, new TypeWrapper(action.GetType()), exception);
                    throw;
                }
                context.HookExtension?.OnAfterTestActionAfterTest(context, new TypeWrapper(action.GetType()));
            };
        }
    }
}
