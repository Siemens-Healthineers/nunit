using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateMethodLevelBeforeTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateMethodLevelBeforeTestHooksAttribute));
        }
    }
}
