using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    internal sealed class TestActionLoggingExecutionHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public void ApplyToContext(TestExecutionContext context)
        {
            context.ExecutionHooks.AddBeforeTestActionBeforeTestHandler(hookData =>
            {
                TestLog.LogCurrentMethod($"BeforeTestActionBeforeTestHook({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddAfterTestActionBeforeTestHandler(hookData =>
            {
                TestLog.LogCurrentMethod($"AfterTestActionBeforeTestHook({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddBeforeTestActionAfterTestHandler(hookData =>
            {
                TestLog.LogCurrentMethod($"BeforeTestActionAfterTestHook({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
            });
            context.ExecutionHooks.AddAfterTestActionAfterTestHandler(hookData =>
            {
                TestLog.LogCurrentMethod($"AfterTestActionAfterTestHook({(hookData.Context.Test.IsSuite ? "Suite" : "Test")})");
            });
        }
    }
}
