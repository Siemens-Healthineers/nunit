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
                if (context.ExecutionHooksEnabled)
                {
                    var hookedMethodInfo = new MethodWrapper(action.GetType(), "BeforeTest");
                    try
                    {
                        context.ExecutionHooks.OnBeforeTestActionBeforeTest(context, hookedMethodInfo);

                        action.BeforeTest(Test);
                    }
                    catch (Exception ex)
                    {
                        context.ExecutionHooks.OnAfterTestActionBeforeTest(context, hookedMethodInfo, ex);
                        throw;
                    }
                    context.ExecutionHooks.OnAfterTestActionBeforeTest(context, hookedMethodInfo);
                }
                else
                {
                    action.BeforeTest(Test);
                }
            };

            AfterTest = context =>
            {
                if (context.ExecutionHooksEnabled)
                {
                    var hookedMethodInfo = new MethodWrapper(action.GetType(), "AfterTest");
                    try
                    {
                        context.ExecutionHooks.OnBeforeTestActionAfterTest(context, hookedMethodInfo);

                        action.AfterTest(Test);
                    }
                    catch (Exception ex)
                    {
                        context.ExecutionHooks.OnAfterTestActionAfterTest(context, hookedMethodInfo, ex);
                        throw;
                    }
                    context.ExecutionHooks.OnAfterTestActionAfterTest(context, hookedMethodInfo);
                }
                else
                {
                    action.AfterTest(Test);
                }
            };
        }
    }
}
