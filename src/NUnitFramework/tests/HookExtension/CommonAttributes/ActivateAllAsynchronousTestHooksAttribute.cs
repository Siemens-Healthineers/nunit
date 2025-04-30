// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateAllAsynchronousTestHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeAnySetUpsHook.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnySetUpsHook);
            });

            context?.HookExtension?.AfterAnySetUpsHook.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnySetUpsHook);
            });

            context?.HookExtension?.BeforeTestHook.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });

            context?.HookExtension?.AfterTestHook.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
            });

            context?.HookExtension?.BeforeAnyTearDownsHook.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnyTearDownsHook);
            });

            context?.HookExtension?.AfterAnyTearDownsHook.AddAsyncHandler(async (sender, eventArgs) =>
            {
                await Task.Delay(1000);
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnyTearDownsHook);
            });
        }
    }
}
