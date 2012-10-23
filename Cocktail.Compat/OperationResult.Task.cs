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

using System.Threading.Tasks;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public partial class OperationResult : IAwaitable
    {
        private TaskCompletionSource<bool> _tcs;

        #region IAwaitable Members

        /// <summary>
        ///   Returns a Task for the current OperationResult.
        /// </summary>
        public Task AsTask()
        {
            if (_tcs != null) return _tcs.Task;

            _tcs = new TaskCompletionSource<bool>();
            _asyncOp.WhenCompleted(
                args =>
                    {
                        if (args.Cancelled)
                            _tcs.SetCanceled();
                        else if (args.Error != null && !args.IsErrorHandled)
                        {
                            args.IsErrorHandled = true;
                            _tcs.SetException(args.Error);
                        }
                        else
                            _tcs.SetResult(true);
                    });

            return _tcs.Task;
        }

        #endregion

        /// <summary>
        ///   Implicitly converts the current OperationResult to type <see cref="Task" />
        /// </summary>
        /// <param name="operation"> The OperationResult to be converted. </param>
        public static implicit operator Task(OperationResult operation)
        {
            return operation.AsTask();
        }

        /// <summary>
        ///   Implicitly converts the current <see cref="Task"/> to type <see cref="OperationResult" />
        /// </summary>
        /// <param name="task"> The Task to be converted. </param>
        public static implicit operator OperationResult(Task task)
        {
            return task.AsOperationResult();
        }
    }

    public abstract partial class OperationResult<T> : IAwaitable<T>
    {
        private TaskCompletionSource<T> _tcs;

        #region IAwaitable<T> Members

        /// <summary>
        ///   Returns a Task&lt;T&gt; for the current OperationResult.
        /// </summary>
        public new Task<T> AsTask()
        {
            if (_tcs != null) return _tcs.Task;

            _tcs = new TaskCompletionSource<T>();
            ((INotifyCompleted) this).WhenCompleted(
                args =>
                    {
                        if (args.Cancelled)
                            _tcs.SetCanceled();
                        else if (args.Error != null && !args.IsErrorHandled)
                        {
                            args.IsErrorHandled = true;
                            _tcs.SetException(args.Error);
                        }
                        else
                            _tcs.SetResult(args.Error == null ? Result : default(T));
                    });

            return _tcs.Task;
        }

        #endregion

        /// <summary>
        ///   Implicitly converts the current OperationResult to type <see cref="Task" />
        /// </summary>
        /// <param name="operation"> The OperationResult to be converted. </param>
        public static implicit operator Task(OperationResult<T> operation)
        {
            return operation.AsTask();
        }

        /// <summary>
        ///   Implicitly converts the current OperationResult to type <see cref="Task{T}" />
        /// </summary>
        /// <param name="operation"> The OperationResult to be converted. </param>
        public static implicit operator Task<T>(OperationResult<T> operation)
        {
            return operation.AsTask();
        }

        /// <summary>
        ///   Implicitly converts the current <see cref="Task{T}"/> to type <see cref="OperationResult{T}" />
        /// </summary>
        /// <param name="task"> The Task to be converted. </param>
        public static implicit operator OperationResult<T>(Task<T> task)
        {
            return task.AsOperationResult();
        }
    }
}