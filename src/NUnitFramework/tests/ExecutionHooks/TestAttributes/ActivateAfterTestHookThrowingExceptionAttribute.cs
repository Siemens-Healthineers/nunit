using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateAfterTestHookThrowingExceptionAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookThrowingExceptionAttribute));
            throw new Exception("After test hook crashed!!");
        }
    }
}
