// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal static class HookIdentifiers
    {
        internal static readonly string Hook = "_Hook";

        internal static readonly string AfterTestHook = $"AfterTestHook{Hook}";
        internal static readonly string BeforeAnySetUpsHook = $"BeforeAnySetUpsHook{Hook}";
        internal static readonly string AfterAnySetUpsHook = $"AfterAnySetUpsHook{Hook}";
        internal static readonly string BeforeTestHook = $"BeforeTestHook{Hook}";
        internal static readonly string BeforeAnyTearDownsHook = $"BeforeAnyTearDownsHook{Hook}";
        internal static readonly string AfterAnyTearDownsHook = $"AfterAnyTearDownsHook{Hook}";
    }
}
