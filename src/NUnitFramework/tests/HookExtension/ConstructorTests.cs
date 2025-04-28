// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.HookExtension
{
    internal class HookExtensionConstructorTests
    {
        [Test]
        public void CopyCtor_CreateNewHookExtension_InvocationListShouldBeEmpty()
        {
            var hookExt = new Framework.Internal.HookExtensions.HookExtension();

            Assert.Multiple(() =>
            {
                Assert.That(hookExt.BeforeTestHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterTestHook.GetHandlers().Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void CopyCtor_CallMultipleTimes_ShallNotIncreaseInvocationList()
        {
            var hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension();
            hookExt.AfterTestHook.AddHandler((sender, args) => { });
            
            hookExt = new Framework.Internal.HookExtensions.HookExtension(hookExt);
            hookExt = new Framework.Internal.HookExtensions.HookExtension(hookExt);
            hookExt = new Framework.Internal.HookExtensions.HookExtension(hookExt);

            // all others shall stay empty
            Assert.Multiple(() =>
            {
                Assert.That(hookExt.AfterTestHook.GetHandlers().Count, Is.EqualTo(1));
                Assert.That(hookExt.BeforeTestHook.GetHandlers().Count, Is.EqualTo(0));
            });
        }
    }
}
