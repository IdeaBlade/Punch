using Caliburn.Micro;
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
            var eventAggregator = new EventAggregator();
            var handler = new TestEventHandler();
            Assert.IsTrue(eventAggregator.IsHandler(handler, typeof(string)), "Should be a message handler");
        }

        [TestMethod]
        public void ShouldBePolymorphicMessageHandler()
        {
            var eventAggregator = new EventAggregator();
            var handler = new TestEventHandler();
            Assert.IsTrue(eventAggregator.IsHandler(handler, typeof(TestMessage)), "Should be a message handler");
        }

        [TestMethod]
        public void ShoudForwardMessage()
        {
            var eventAggregator = new EventAggregator();
            var handler = new TestEventHandler();
            eventAggregator.Forward(handler, "Hello");
            Assert.IsTrue(handler.StringHandled, "Message should have been handled");
        }

        [TestMethod]
        public void ShouldForwardPolymorphicMessage()
        {
            var eventAggregator = new EventAggregator();
            var handler = new TestEventHandler();
            var message = new TestMessage();
            eventAggregator.Forward(handler, message);
            Assert.IsTrue(handler.IMessageHandled, "Message should have been handled");
        }
    }
}