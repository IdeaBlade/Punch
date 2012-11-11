using System;
using System.Reflection;

namespace Cocktail
{
    public static class EventHandlerUtils
    {
        // Methods
        public static EventHandler<E> MakeWeak<E>(this EventHandler<E> eventHandler, UnregisterCallback<E> unregister) where E : EventArgs
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }
            MethodInfo method = eventHandler.Method;
            if (method.IsStatic || (eventHandler.Target == null))
            {
                throw new ArgumentException("Only instance methods are supported.", "eventHandler");
            }
            var handler = (IWeakEventHandler<E>)typeof(WeakEventHandler<,>).MakeGenericType(new Type[] { method.DeclaringType, typeof(E) }).GetConstructor(new[] { typeof(EventHandler<E>), typeof(UnregisterCallback<E>) }).Invoke(new object[] { eventHandler, unregister });
            return handler.Handler;
        }
    }

 

}
