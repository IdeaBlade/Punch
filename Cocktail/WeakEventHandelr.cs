using System;

namespace Cocktail
{
    public delegate void UnregisterCallback<E>(EventHandler<E> eventHandler) where E : EventArgs;

    public class WeakEventHandler<T, E> : IWeakEventHandler<E>
        where T : class
        where E : EventArgs
    {
        private delegate void OpenEventHandler(T target, Object sender, E eventArgs);

        private readonly EventHandler<E> _handler;
        private readonly OpenEventHandler _openHandler;
        private readonly WeakReference _targetRef;
        private UnregisterCallback<E> _unregister;

        public EventHandler<E> Handler
        {
            get
            {
                return _handler;
            }
        }

        public WeakEventHandler(EventHandler<E> eventHandler, UnregisterCallback<E> unregister)
        {
            _targetRef = new WeakReference(eventHandler.Target);
            _openHandler = (OpenEventHandler)Delegate.CreateDelegate(typeof(OpenEventHandler), null, eventHandler.Method);
            _handler = Invoke;
            _unregister = unregister;
        }

        public void Invoke(object sender, E e)
        {
            var target = (T)_targetRef.Target;
            if (target != null)
            {
                _openHandler(target, sender, e);
            }
            else if (_unregister != null)
            {
                _unregister(_handler);
                _unregister = null;
            }
        }

        public static implicit operator EventHandler<E>(WeakEventHandler<T, E> weh)
        {
            return weh._handler;
        }
    }
}
