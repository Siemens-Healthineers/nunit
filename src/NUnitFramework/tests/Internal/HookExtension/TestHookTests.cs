// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.HookExtensions;

namespace NUnit.Framework.Tests.Internal.HookExtension
{
    public class TestHookTests
    {
        [Test]
        public void Invoke_HookWithAsyncHandlerThrowingException_AggregatedExceptionIsThrown()
        {
            var testHook = new TestHook<MethodHookEventArgs>();
            var exception = new Exception("Test exception");
            var testMethodEventArgs = new MethodHookEventArgs(null, null);
            testHook.AddAsyncHandler(async (sender, args) => throw exception);
            Assert.Throws<AggregateException>(() => testHook.InvokeHandlers(this, testMethodEventArgs).Wait());
        }

        [Test]
        public void Invoke_HookWithSyncHandlerThrowingException_AggregatedExceptionIsThrown()
        {
            var testHook = new TestHook<MethodHookEventArgs>();
            var exception = new Exception("Test exception");
            var testMethodEventArgs = new MethodHookEventArgs(null, null);
            testHook.AddHandler((sender, args) => throw exception);
            Assert.Throws<AggregateException>(() => testHook.InvokeHandlers(this, testMethodEventArgs).Wait());
        }
    }
}
