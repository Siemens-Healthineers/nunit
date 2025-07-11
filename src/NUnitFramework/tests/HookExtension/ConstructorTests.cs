// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.HookExtension
{
    internal class HookExtensionConstructorTests
    {
        [Test]
        public void CopyCtor_CreateNewHookExtension_InvocationListShouldBeEmpty()
        {
            var hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension();

            Assert.Multiple(() =>
            {
                Assert.That(hookExt.BeforeAnySetUpsHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUpsHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTestHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterTestHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDownsHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDownsHook.GetHandlers().Count, Is.EqualTo(0));

                Assert.That(hookExt.BeforeAnySetUpsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUpsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTestHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterTestHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDownsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDownsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void CopyCtor_CallMultipleTimes_ShallNotIncreaseInvocationList()
        {
            var hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension();
            hookExt.AfterTestHook.AddHandler((sender, args) => { });
            //hookExt.AfterTestHook.AddAsyncHandler(async (sender, args) => { await Task.Delay(1); });

            hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension(hookExt);
            hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension(hookExt);
            hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension(hookExt);

            // initially assigned handlers shall be copied
            Assert.Multiple(() =>
            {
                Assert.That(hookExt.AfterTestHook.GetHandlers().Count, Is.EqualTo(1));
                //Assert.That(hookExt.AfterTestHook.GetAsyncHandlers().Count, Is.EqualTo(1));
            });

            // all others shall stay empty
            Assert.Multiple(() =>
            {
                Assert.That(hookExt.BeforeAnySetUpsHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUpsHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTestHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDownsHook.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDownsHook.GetHandlers().Count, Is.EqualTo(0));

                //Assert.That(hookExt.BeforeAnySetUpsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                //Assert.That(hookExt.AfterAnySetUpsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                //Assert.That(hookExt.BeforeTestHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                //Assert.That(hookExt.BeforeAnyTearDownsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
                //Assert.That(hookExt.AfterAnyTearDownsHook.GetAsyncHandlers().Count, Is.EqualTo(0));
            });
        }
    }
}
