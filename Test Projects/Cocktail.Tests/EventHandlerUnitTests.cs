//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using Cocktail.Tests.Helpers;

#if !NETFX_CORE
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using System.Composition;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

namespace Cocktail.Tests
{
    [TestClass]
    public class EventHandlerUnitTests : CocktailTestBase
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