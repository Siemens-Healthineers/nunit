using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class ActivateAfterTestHookAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateAfterTestHookAttribute));
        }
    }
}
