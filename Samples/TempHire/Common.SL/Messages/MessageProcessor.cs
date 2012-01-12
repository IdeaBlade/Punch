using Caliburn.Micro;
using Cocktail;

namespace Common.Messages
{
    public abstract class MessageProcessor<T> : IMessageProcessor, IHandle<T>
    {
        protected MessageProcessor()
        {
            Composition.GetInstance<IEventAggregator>().Subscribe(this);
        }

        #region IHandle<T> Members

        public abstract void Handle(T message);

        #endregion
    }
}