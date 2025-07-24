using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateAllSynchronousTestHooksAttribute : ExecutionHookAttribute
    {
        public override void BeforeEverySetUpHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeEverySetUpHook);
        }

        public override void AfterEverySetUpHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterEverySetUpHook);
        }

        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook);
        }

        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook);
        }

        public override void BeforeEveryTearDownHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeEveryTearDownHook);
        }

        public override void AfterEveryTearDownHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterEveryTearDownHook);
        }
    }
}
