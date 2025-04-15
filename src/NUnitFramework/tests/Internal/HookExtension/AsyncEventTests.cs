// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal.HookExtensions;

namespace NUnit.Framework.Tests.Internal.HookExtension
{
    public class AsyncEventTests
    {
        [Test]
        public void Invoke_AsyncEventThrowingException_AggregatedExceptionIsThrown()
        {
            var asyncEvent = new AsyncEvent<TestHookEventArgs>();
            var exception = new Exception("Test exception");
            var testMethodEventArgs = new TestHookEventArgs(null);
            asyncEvent.AddAsyncHandler(async (sender, args) => throw exception);
            Assert.Throws<AggregateException>(() => asyncEvent.Invoke(this, testMethodEventArgs).Wait());
        }

        [Test]
        public void Invoke_SyncEventThrowingException_AggregatedExceptionIsThrown()
        {
            var syncEvent = new AsyncEvent<TestHookEventArgs>();
            var exception = new Exception("Test exception");
            var testMethodEventArgs = new TestHookEventArgs(null);
            syncEvent.AddHandler((sender, args) => throw exception);
            Assert.Throws<AggregateException>(() => syncEvent.Invoke(this, testMethodEventArgs).Wait());
        }
    }
}
