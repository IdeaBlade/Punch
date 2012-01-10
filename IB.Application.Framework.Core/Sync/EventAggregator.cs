//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaBlade.Application.Framework.Core.Sync
{
    /// <summary>
    /// Internal use.
    /// </summary>
    internal interface IEventAggregator
    {
        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref="IHandle{T}"/>
        /// </summary>
        /// <param name="instance">The instance to subscribe for event publication.</param>
        void Subscribe(object instance);

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        /// <param name="instance">The instance to unsubscribe.</param>
        void Unsubscribe(object instance);

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="T">The type of message being published.</typeparam>
        /// <param name="message">The message instance.</param>
        void Publish<T>(T message);
    }

    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    internal class EventAggregator : IEventAggregator
    {
        //static readonly ILog Log = LogManager.GetLog(typeof(EventAggregator));
        readonly List<WeakReference> _subscribers = new List<WeakReference>();

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref="IHandle{T}"/>
        /// </summary>
        /// <param name="instance">The instance to subscribe for event publication.</param>
        public void Subscribe(object instance)
        {
            lock (_subscribers)
            {
                if (_subscribers.Any(reference => reference.Target == instance))
                    return;

                //Log.Info("Subscribing {0}.", instance);
                _subscribers.Add(new WeakReference(instance));
            }
        }

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        /// <param name="instance">The instance to unsubscribe.</param>
        public void Unsubscribe(object instance)
        {
            lock (_subscribers)
            {
                var found = _subscribers
                    .FirstOrDefault(reference => reference.Target == instance);

                if (found != null)
                    _subscribers.Remove(found);
            }
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="TMessage">The type of message being published.</typeparam>
        /// <param name="message">The message instance.</param>
        public void Publish<TMessage>(TMessage message)
        {
            WeakReference[] toNotify;
            lock (_subscribers)
                toNotify = _subscribers.ToArray();

            Execute.OnUIThread(() =>
            {
                //Log.Info("Publishing {0}.", message);
                var dead = new List<WeakReference>();

                foreach (var reference in toNotify)
                {
                    var target = reference.Target as IHandle<TMessage>;

                    if (target != null)
                        target.Handle(message);
                    else if (!reference.IsAlive)
                        dead.Add(reference);
                }
                if (dead.Count > 0)
                {
                    lock (_subscribers)
                        dead.Apply(x => _subscribers.Remove(x));
                }
            });
        }
    }
}