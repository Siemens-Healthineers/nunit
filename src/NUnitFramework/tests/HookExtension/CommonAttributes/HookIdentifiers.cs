// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.HookExtension.CommonAttributes
{
    internal static class HookIdentifiers
    {
        internal static readonly string Hook = "_Hook";

        internal static readonly string AfterTestHook = $"AfterTestHook{Hook}";
        internal static readonly string BeforeTestHook = $"BeforeTestHook{Hook}";
    }
}
