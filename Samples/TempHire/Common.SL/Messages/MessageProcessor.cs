using Caliburn.Micro;
using IdeaBlade.Application.Framework.Core.Composition;

namespace Common.Messages
{
    public abstract class MessageProcessor<T> : IMessageProcessor, IHandle<T>
    {
        protected MessageProcessor()
        {
            CompositionHelper.GetInstance<IEventAggregator>().Subscribe(this);
        }

        #region IHandle<T> Members

        public abstract void Handle(T message);

        #endregion
    }
}