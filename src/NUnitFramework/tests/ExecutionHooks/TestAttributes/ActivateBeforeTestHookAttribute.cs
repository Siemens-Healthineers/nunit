using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class ActivateBeforeTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateBeforeTestHookAttribute));
        }
    }
}
