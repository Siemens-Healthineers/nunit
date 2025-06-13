// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.ExecutionHooks
{
    /// <summary>
    /// Event that supports both synchronous and asynchronous handlers.
    /// </summary>
    internal sealed class TestHook<TEventArgs>
    {
        private readonly List<EventHandler<MethodHookEventArgs>> _handlers;

        internal int Count
        {
            get
            {
                lock (_handlers)
                {
                    return _handlers.Count;
                }
            }
        }

        public TestHook()
        {
            _handlers = new List<EventHandler<MethodHookEventArgs>>();
        }

        public TestHook(TestHook<TEventArgs> source)
        {
            _handlers = new List<EventHandler<MethodHookEventArgs>>(source._handlers);
        }

        internal void AddHandler(EventHandler<MethodHookEventArgs> handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        internal void InvokeHandlers(object? sender, MethodHookEventArgs e)
        {
            foreach (var handler in GetHandlers())
            {
                handler(sender, e);
            }
        }

        private IReadOnlyList<EventHandler<MethodHookEventArgs>> GetHandlers()
        {
            lock (_handlers)
                return _handlers.ToArray();
        }
    }
}
