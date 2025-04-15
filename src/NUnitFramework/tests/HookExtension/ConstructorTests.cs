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
                Assert.That(hookExt.BeforeAnySetUps.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUps.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTest.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterTest.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDowns.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDowns.GetHandlers().Count, Is.EqualTo(0));

                Assert.That(hookExt.BeforeAnySetUps.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUps.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTest.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterTest.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDowns.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDowns.GetAsyncHandlers().Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void CopyCtor_CallMultipleTimes_ShallNotIncreaseInvocationList()
        {
            var hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension();
            hookExt.AfterTest.AddHandler((sender, args) => { });
            hookExt.AfterTest.AddAsyncHandler(async (sender, args) => { });

            hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension(hookExt);
            hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension(hookExt);
            hookExt = new NUnit.Framework.Internal.HookExtensions.HookExtension(hookExt);

            // initially assigned handlers shall be copied
            Assert.Multiple(() =>
            {
                Assert.That(hookExt.AfterTest.GetHandlers().Count, Is.EqualTo(1));
                Assert.That(hookExt.AfterTest.GetAsyncHandlers().Count, Is.EqualTo(1));
            });

            // all others shall stay empty
            Assert.Multiple(() =>
            {
                Assert.That(hookExt.BeforeAnySetUps.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUps.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTest.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDowns.GetHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDowns.GetHandlers().Count, Is.EqualTo(0));

                Assert.That(hookExt.BeforeAnySetUps.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnySetUps.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeTest.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.BeforeAnyTearDowns.GetAsyncHandlers().Count, Is.EqualTo(0));
                Assert.That(hookExt.AfterAnyTearDowns.GetAsyncHandlers().Count, Is.EqualTo(0));
            });
        }
    }
}
