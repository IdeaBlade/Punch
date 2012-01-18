//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
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