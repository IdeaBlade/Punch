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