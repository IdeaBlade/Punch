// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System;
using System.Collections.Generic;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public static partial class Composition
    {
        private static readonly Dictionary<string, XapDownloadOperation> XapDownloadOperations =
            new Dictionary<string, XapDownloadOperation>();

        /// <summary>Asynchronously downloads a XAP file and adds all exported parts to the catalog.</summary>
        /// <param name="relativeUri">The relative URI for the XAP file to be downloaded.</param>
        /// <param name="onSuccess">User callback to be called when operation completes successfully.</param>
        /// <param name="onFail">User callback to be called when operation completes with an error.</param>
        /// <returns>Returns a handle to the download operation.</returns>
        public static OperationResult AddXapAsync(string relativeUri, Action onSuccess = null, Action<Exception> onFail = null)
        {
            XapDownloadOperation operation;
            if (XapDownloadOperations.TryGetValue(relativeUri, out operation) && !operation.HasError)
                return operation.AsOperationResult();

            var op = XapDownloadOperations[relativeUri] = new XapDownloadOperation(relativeUri);
            op.WhenCompleted(
                args =>
                {
                    if (args.Error == null && onSuccess != null)
                        onSuccess();

                    if (args.Error != null && onFail != null)
                    {
                        args.IsErrorHandled = true;
                        onFail(args.Error);
                    }
                });
            return op.AsOperationResult();
        }
    }

    internal class XapDownloadOperation : INotifyCompleted
    {
        private readonly DynamicXap _xap;
        private XapDownloadCompletedEventArgs _completedEventArgs;
        private Action<INotifyCompletedArgs> _notifyCompletedActions;

        public XapDownloadOperation(string xapUri)
        {
            _xap = new DynamicXap(new Uri(xapUri, UriKind.Relative));
            _xap.Loaded += (s, args) => XapDownloadCompleted(args);
        }

        private void XapDownloadCompleted(DynamicXapLoadedEventArgs args)
        {
            Exception error = null;
            if (!args.HasError)
            {
                Composition.IsRecomposing = true;
                try
                {
                    CompositionHost.Add(_xap);
                }
                catch (Exception e)
                {
                    error = e;
                }
                finally
                {
                    Composition.IsRecomposing = false;
                }
            }

            _completedEventArgs = new XapDownloadCompletedEventArgs(args.Cancelled, args.Error ?? error);

            CallCompletedActions();
        }

        protected void CallCompletedActions()
        {
            Action<INotifyCompletedArgs> actions = _notifyCompletedActions;
            _notifyCompletedActions = null;
            if (actions == null) return;
            actions(_completedEventArgs);
        }

        #region Implementation of INotifyCompleted

        /// <summary>
        /// Action to be performed when the asynchronous operation completes.
        /// </summary>
        /// <param name="completedAction"/>
        public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
        {
            if (completedAction == null) return;
            if (_completedEventArgs != null)
            {
                completedAction(_completedEventArgs);
                return;
            }
            _notifyCompletedActions =
                (Action<INotifyCompletedArgs>)Delegate.Combine(_notifyCompletedActions, completedAction);
        }

        /// <summary>
        /// Returns whether the operation completed successfully
        /// </summary>
        public bool CompletedSuccessfully
        {
            get { return _completedEventArgs != null && !_completedEventArgs.HasError && !_completedEventArgs.Cancelled; }
        }

        /// <summary>
        /// Returns whether the operation failed.
        /// </summary>
        public bool HasError
        {
            get { return _completedEventArgs != null && _completedEventArgs.HasError; }
        }

        /// <summary>
        /// The exception if the action failed.
        /// </summary>
        public Exception Error
        {
            get { return _completedEventArgs != null ? _completedEventArgs.Error : null; }
        }

        #endregion
    }

    internal class XapDownloadCompletedEventArgs : EventArgs, INotifyCompletedArgs
    {
        private readonly bool _cancelled;
        private readonly Exception _error;
        //private readonly DynamicXapLoadedEventArgs _dynamicXapLoadedEventArgs;

        public XapDownloadCompletedEventArgs(bool cancelled, Exception error)
        {
            _cancelled = cancelled;
            _error = error;
            //_dynamicXapLoadedEventArgs = dynamicXapLoadedEventArgs;
        }

        #region Implementation of INotifyCompletedArgs

        /// <summary>
        /// The exception if the action failed.
        /// </summary>
        public Exception Error
        {
            get { return _error; /*_dynamicXapLoadedEventArgs.Error;*/ }
        }

        /// <summary>
        /// Whether the action was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get { return _cancelled; /*_dynamicXapLoadedEventArgs.Cancelled;*/ }
        }

        /// <summary>
        /// Returns whether the operation failed.
        /// </summary>
        public bool HasError { get { return _error != null; /*_dynamicXapLoadedEventArgs.HasError;*/ } }

        /// <summary>
        /// Whether the error was handled.
        /// </summary>
        public bool IsErrorHandled { get; set; }

        #endregion
    }
}