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
using System.Threading.Tasks;
using Caliburn.Micro;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public static partial class OperationFns
    {
        /// <summary>
        ///   Returns an implementation of <see cref="OperationResult" /> that wraps <see cref="Task" /> .
        /// </summary>
        /// <param name="task"> The task to be wrapped. </param>
        /// <returns> OperationResult encapsulating the provided <see cref="Task" /> . </returns>
        public static OperationResult AsOperationResult(this Task task)
        {
            return new TaskOperationResult(task);
        }

        /// <summary>
        ///   Returns an implementation of <see cref="OperationResult" /> that wraps <see cref="Task{T}" /> .
        /// </summary>
        /// <param name="task"> The task to be wrapped. </param>
        /// <returns> OperationResult encapsulating the provided <see cref="Task{T}" /> . </returns>
        public static OperationResult<T> AsOperationResult<T>(this Task<T> task)
        {
            return new TaskOperationResult<T>(task);
        }
    }

    internal class TaskOperationResult : OperationResult
    {
        public TaskOperationResult(Task task) : base(new TaskNotifyCompleted(task))
        {
        }
    }

    internal class TaskOperationResult<T> : OperationResult<T>
    {
        private readonly Task<T> _task;

        public TaskOperationResult(Task<T> task) : base(new TaskNotifyCompleted(task))
        {
            _task = task;
        }

        public override T Result
        {
            get { return _task.Result; }
        }
    }

    internal class TaskNotifyCompleted : INotifyCompleted
    {
        private TaskNotifyCompletedArgs _completedArgs;
        private Action<TaskNotifyCompletedArgs> _notifyActions;

        public TaskNotifyCompleted(Task task)
        {
            task.ContinueWith(t => Execute.OnUIThread(() => OnCompleted(t)));
        }

        #region INotifyCompleted Members

        public void WhenCompleted(Action<INotifyCompletedArgs> completedAction)
        {
            if (completedAction == null) return;
            if (_completedArgs != null)
            {
                completedAction(_completedArgs);
                return;
            }
            _notifyActions = (Action<TaskNotifyCompletedArgs>) Delegate.Combine(_notifyActions, completedAction);
        }

        #endregion

        private void OnCompleted(Task task)
        {
            _completedArgs = new TaskNotifyCompletedArgs(task);
            var actions = _notifyActions;
            _notifyActions = null;
            if (actions == null) return;
            actions(_completedArgs);
        }
    }

    internal class TaskNotifyCompletedArgs : INotifyCompletedArgs
    {
        private readonly Task _task;

        public TaskNotifyCompletedArgs(Task task)
        {
            _task = task;
        }

        #region INotifyCompletedArgs Members

        public Exception Error
        {
            get { return _task.IsFaulted ? _task.Exception : null; }
        }

        public bool Cancelled
        {
            get { return _task.IsCanceled; }
        }

        public bool IsErrorHandled { get; set; }

        #endregion
    }
}