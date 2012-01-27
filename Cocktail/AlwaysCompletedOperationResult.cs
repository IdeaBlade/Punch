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

using System;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>A singleton implementation of an always completed result.</summary>
    public class AlwaysCompletedOperationResult : OperationResult
    {
        private static AlwaysCompletedOperationResult _instance;

        /// <summary>
        /// Constructs a new AlwaysCompletedOperationResult instance.
        /// </summary>
        protected AlwaysCompletedOperationResult() : base(AlwaysCompleted.Instance)
        {
        }

        /// <summary>Returns the singleton instance.</summary>
        /// <value>The AlwaysCompletedOperationResult instance.</value>
        public static AlwaysCompletedOperationResult Instance
        {
            get { return _instance ?? (_instance = new AlwaysCompletedOperationResult()); }
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