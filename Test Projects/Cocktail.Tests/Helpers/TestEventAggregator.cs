using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Action = System.Action;

namespace Cocktail.Tests.Helpers
{
    [Export(typeof (IEventAggregator))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TestEventAggregator : IEventAggregator
    {
        private static IEventAggregator _eventAggregator;

        private IEventAggregator EventAggregator
        {
            get { return _eventAggregator ?? (_eventAggregator = new EventAggregator()); }
        }

        #region IEventAggregator Members

        Action<Action> IEventAggregator.PublicationThreadMarshaller
        {
            get { return EventAggregator.PublicationThreadMarshaller; }
            set { EventAggregator.PublicationThreadMarshaller = value; }
        }

        void IEventAggregator.Publish(object message, Action<Action> marshal)
        {
            EventAggregator.Publish(message, marshal);
        }

        void IEventAggregator.Publish(object message)
        {
            EventAggregator.Publish(message);
        }

        void IEventAggregator.Subscribe(object instance)
        {
            EventAggregator.Subscribe(instance);
        }

        void IEventAggregator.Unsubscribe(object instance)
        {
            EventAggregator.Unsubscribe(instance);
        }

        #endregion

        public static void Reset()
        {
            _eventAggregator = null;
        }
    }
}