//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
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

namespace IdeaBlade.Application.Framework.Core.Sync
{
    /// <summary>
    /// A marker interface for classes that subscribe to messages.
    /// </summary>
    internal interface IHandle { }

    /// <summary>
    /// Denotes a class which can handle a particular type of message.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to handle.</typeparam>
#if WP7
    internal interface IHandle<TMessage> : IHandle
#else
    internal interface IHandle<in TMessage> : IHandle
#endif
    {
        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Handle(TMessage message);
    }
}