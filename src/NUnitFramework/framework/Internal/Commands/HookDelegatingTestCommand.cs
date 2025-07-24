// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    internal sealed class HookDelegatingTestCommand : DelegatingTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HookDelegatingTestCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner test command to delegate to.</param>
        public HookDelegatingTestCommand(TestCommand innerCommand) : base(innerCommand)
        {
        }

        /// <summary>
        /// Executes the test command within the provided context
        /// </summary>
        /// <param name="context">The test execution context.</param>
        /// <returns>The result of the test execution.</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            //Ask Manfred about proposal for handling this possible null value
            IMethodInfo hookedMethodInfo = context.CurrentTest.Method;
            // IMethodInfo hookedMethodInfo = context.CurrentTest.Method ?? new MethodWrapper(GetType(), nameof(Execute));

            try
            {
                context.ExecutionHooks.OnBeforeTest(context, hookedMethodInfo);
                innerCommand.Execute(context);
            }
            catch (Exception ex)
            {
                context.ExecutionHooks.OnAfterTest(context, hookedMethodInfo, ex);
                throw;
            }
            context.ExecutionHooks.OnAfterTest(context, hookedMethodInfo);

            return context.CurrentResult;
        }
    }
}
