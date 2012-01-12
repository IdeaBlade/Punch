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

using System;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>A singleton implementation of an always completed result.</summary>
    public class AlwaysCompletedOperation : AsyncOperation
    {
        private static AlwaysCompletedOperation _instance;

        /// <summary>
        /// Constructs a new AlwaysCompletedOperation instance.
        /// </summary>
        protected AlwaysCompletedOperation() : base(AlwaysCompleted.Instance)
        {
        }

        /// <summary>Returns the singleton instance.</summary>
        /// <value>The AlwaysCompletedOperation instance.</value>
        public static AlwaysCompletedOperation Instance
        {
            get { return _instance ?? (_instance = new AlwaysCompletedOperation()); }
        }
    }

    /// <summary>A singleton implementation of an always completed operation.</summary>
    internal class AlwaysCompleted : INotifyCompleted
    {
        private static AlwaysCompleted _instance;

        /// <summary>Returns the singleton instance.</summary>
        /// <value>The AlwaysCompleted instance.</value>
        public static AlwaysCompleted Instance
        {
            get { return _instance ?? (_instance = new AlwaysCompleted()); }
        }

        #region INotifyCompleted Members

        /// <summary>Immediatley calls the completedAction.</summary>
        /// <param name="completedAction">Callback to be called.</param>
        public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
        {
            completedAction(new AlwaysCompletedArgs());
        }

        #endregion

        #region Nested type: AlwaysCompletedArgs

        private class AlwaysCompletedArgs : INotifyCompletedArgs
        {
            #region INotifyCompletedArgs Members

            public Exception Error
            {
                get { return null; }
            }

            public bool Cancelled
            {
                get { return false; }
            }

            public bool IsErrorHandled { get; set; }

            #endregion
        }

        #endregion
    }
}