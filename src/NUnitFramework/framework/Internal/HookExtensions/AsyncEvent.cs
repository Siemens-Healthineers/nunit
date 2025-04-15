// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnit.Framework.Internal.HookExtensions
{
    /// <summary>
    /// Event that supports both synchronous and asynchronous handlers.
    /// </summary>
    public sealed class AsyncEvent<TEventArgs>
    {
        private readonly List<Delegate> _handlers = new();
        private readonly List<Delegate> _asyncHandlers = new();

        /// <summary>
        /// Adds a synchronous handler to the event.
        /// </summary>
        /// <param name="handler">The event handler to be attached to the event.</param>
        public void AddHandler(EventHandler<TEventArgs> handler)
        {
            lock (_handlers)
                _handlers.Add(handler);
        }

        /// <summary>
        /// Adds an asynchronous handler to the event.
        /// </summary>
        /// <param name="asyncHandler">The event handler to be attached to the event.</param>
        public void AddAsyncHandler(AsyncEventHandler<TEventArgs> asyncHandler)
        {
            lock (_asyncHandlers)
                _asyncHandlers.Add(asyncHandler);
        }

        internal IReadOnlyList<Delegate> GetHandlers()
        {
            lock (_handlers)
                return _handlers;
        }

        internal IReadOnlyList<Delegate> GetAsyncHandlers()
        {
            lock (_handlers)
                return _asyncHandlers;
        }

        internal Task Invoke(object? sender, TEventArgs e)
        {
            if (!_handlers.Any() && !_asyncHandlers.Any())
            {
                return Task.CompletedTask;
            }

            var tasks = new List<Task>();

            Delegate[] asyncHandlers;
            lock (_asyncHandlers)
                asyncHandlers = _asyncHandlers.ToArray();

            foreach (var handler in asyncHandlers)
            {
                if (handler is AsyncEventHandler<TEventArgs> asyncHandler)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await asyncHandler(sender, e);
                    }));
                }
            }

            Delegate[] syncHandlers;
            lock (_handlers)
                syncHandlers = _handlers.ToArray();

            foreach (var handler in syncHandlers)
            {
                if (handler is EventHandler<TEventArgs> syncHandler)
                {
                    try
                    {
                        syncHandler(sender, e);
                    }
                    catch (Exception ex)
                    {
                        tasks.Add(Task.FromException(ex));
                    }
                }
            }

            Task taskAll = Task.WhenAll(tasks);
            taskAll.Wait();
            return taskAll;
        }
    }
}
