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

using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>The base view model implementing a message box.</summary>
    /// <remarks>To customize the message box, subclass MessageBoxBase and implement a custom view to match the subclassed view model.</remarks>
    [InheritedExport]
    [PartNotDiscoverable]
    public class MessageBoxBase : Screen
    {
        private string _message;

        static MessageBoxBase()
        {
            ViewLocator.NameTransformer.AddRule("MessageBoxBase$", "MessageBoxView");
        }

        /// <summary>
        /// Returns the message to display.
        /// </summary>
        public string Message
        {
            get { return _message; }
            private set
            {
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        /// <summary>
        /// Initializes and Starts the message box.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <returns></returns>
        public MessageBoxBase Start(string message)
        {
            Message = message;
            return this;
        }
    }
}