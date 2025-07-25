using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class ActivateClassLevelAfterTestHooksAttribute : ExecutionHookAttribute
    {
        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(nameof(ActivateClassLevelAfterTestHooksAttribute));
        }
    }
}
