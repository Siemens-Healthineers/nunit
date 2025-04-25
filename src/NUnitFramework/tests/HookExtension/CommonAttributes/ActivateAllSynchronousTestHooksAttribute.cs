// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.TestsUnderTest;

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal class ActivateAllSynchronousTestHooksAttribute : NUnitAttribute, IApplyToContext
    {
        public virtual void ApplyToContext(TestExecutionContext context)
        {
            context?.HookExtension?.BeforeAnySetUpsHook.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnySetUpsHook);
            });

            context?.HookExtension?.AfterAnySetUpsHook.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnySetUpsHook);
            });

            context?.HookExtension?.BeforeTestHook.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeTestHook);
            });

            context?.HookExtension?.AfterTestHook.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterTestHook);
            });

            context?.HookExtension?.BeforeAnyTearDownsHook.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.BeforeAnyTearDownsHook);
            });

            context?.HookExtension?.AfterAnyTearDownsHook.AddHandler((sender, eventArgs) =>
            {
                TestLog.LogCurrentMethod(HookIdentifiers.AfterAnyTearDownsHook);
            });
        }
    }
}
