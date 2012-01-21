using Caliburn.Micro;

namespace Cocktail.Tests.Helpers
{
    public interface IMessage
    {
    }

    public class TestMessage : IMessage
    {
    }

    public class TestEventHandler : IHandle<string>, IHandle<IMessage>
    {
        public bool StringHandled { get; private set; }

        // ReSharper disable InconsistentNaming
        public bool IMessageHandled { get; private set; }
        // ReSharper restore InconsistentNaming

        #region IHandle<IMessage> Members

        public void Handle(IMessage message)
        {
            IMessageHandled = true;
        }

        #endregion

        #region IHandle<string> Members

        public void Handle(string message)
        {
            StringHandled = true;
        }

        #endregion
    }
}