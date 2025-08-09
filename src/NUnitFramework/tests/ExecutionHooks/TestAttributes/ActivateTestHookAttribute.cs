using System;
using NUnit.Framework.Internal.ExecutionHooks;

namespace NUnit.Framework.Tests.ExecutionHooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class ActivateTestHookAttribute : ExecutionHookAttribute
    {
        public override void BeforeTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.BeforeTestHook + $"({hookData.Context.Test.MethodName})");
        }

        public override void AfterTestHook(HookData hookData)
        {
            TestLog.LogMessage(HookIdentifiers.AfterTestHook + $"({hookData.Context.Test.MethodName})");
        }
    }
}
