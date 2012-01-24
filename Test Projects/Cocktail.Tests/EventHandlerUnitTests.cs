using Cocktail.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cocktail.Tests
{
    [TestClass]
    public class EventHandlerUnitTests
    {
        [TestMethod]
        public void ShouldBeMessageHandler()
        {
            var handler = new TestEventHandler();
            Assert.IsTrue(EventFns.IsHandler(handler, typeof (string)), "Should be a message handler");
        }

        [TestMethod]
        public void ShouldBePolymorphicMessageHandler()
        {
            var handler = new TestEventHandler();
            Assert.IsTrue(EventFns.IsHandler(handler, typeof (TestMessage)), "Should be a message handler");
        }

        [TestMethod]
        public void ShoudForwardMessage()
        {
            var handler = new TestEventHandler();
            EventFns.Forward(handler, "Hello");
            Assert.IsTrue(handler.StringHandled, "Message should have been handled");
        }

        [TestMethod]
        public void ShouldForwardPolymorphicMessage()
        {
            var handler = new TestEventHandler();
            var message = new TestMessage();
            EventFns.Forward(handler, message);
            Assert.IsTrue(handler.IMessageHandled, "Message should have been handled");
        }
    }
}