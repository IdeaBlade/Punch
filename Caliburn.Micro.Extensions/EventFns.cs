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
using System.Reflection;
using IdeaBlade.Core;

namespace Caliburn.Micro.Extensions
{
    /// <summary>A collection of extension methods to provide additional EventAggregator functionality</summary>
    public static class EventFns
    {
        /// <summary>Returns true if the provided object implements IHandle for the given messageType.</summary>
        /// <param name="source">An instance of the EventAggregator class.</param>
        /// <param name="handler">The object to be probed.</param>
        /// <param name="messageType">The type of the message to be handled.</param>
        /// <returns>True if the handler handles the given message type.</returns>
        public static bool IsHandler(this IEventAggregator source, object handler, Type messageType)
        {
            IEnumerable<Type> interfaces = handler.GetType().GetInterfaces()
                .Where(i => typeof (IHandle).IsAssignableFrom(i) && i.IsGenericType);

            return interfaces.Any(i => i.GetGenericArguments()[0].IsAssignableFrom(messageType));
        }

        /// <summary>Forwards a given message to the specified handler. The handler must
        ///    implement the corresponding IHandle interface for the message to be delivered.</summary>
        /// <param name="source">An instance of the EventAggregator class.</param>
        /// <param name="handler">The object to handle the message.</param>
        /// <param name="message">The message to be forwarded.</param>
        public static void Forward(this IEventAggregator source, object handler, object message)
        {
            IEnumerable<Type> interfaces = handler.GetType().GetInterfaces()
                .Where(i => typeof (IHandle).IsAssignableFrom(i) && i.IsGenericType);

            IEnumerable<Type> handlers =
                interfaces.Where(i => i.GetGenericArguments()[0].IsInstanceOfType(message));
            handlers.ForEach(
                @interface =>
                    {
                        MethodInfo method = @interface.GetMethod("Handle");
                        method.Invoke(handler, new[] {message});
                    });
        }

        /// <summary>Raises the given event and then clears all event handlers.</summary>
        /// <param name="event">The event to be raised.</param>
        /// <param name="sender">The object raising the event. This will be the sender.</param>
        /// <param name="args">The event arguments.</param>
        public static void RaiseOnce<T>(ref EventHandler<T> @event, object sender, T args)
            where T : EventArgs
        {
            var handlers = @event;
            @event = null;
            handlers(sender, args);
        }
    }
}