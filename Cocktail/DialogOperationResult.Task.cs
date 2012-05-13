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
using Caliburn.Micro;

namespace Cocktail
{
    public abstract partial class DialogOperationResult<T> : IAwaitable<T>
    {
        private TaskCompletionSource<T> _tcs;

        #region IAwaitable<T> Members

        Task IAwaitable.AsTask()
        {
            return AsTask();
        }

        /// <summary>
        ///   Returns a Task&lt;T&gt; for the current DialogOperationResult.
        /// </summary>
        public Task<T> AsTask()
        {
            if (_tcs != null) return _tcs.Task;

            _tcs = new TaskCompletionSource<T>();
            ((IResult) this).Completed +=
                (sender, args) =>
                    {
                        if (args.WasCancelled)
                            _tcs.SetCanceled();
                        else if (args.Error != null)
                            _tcs.SetException(args.Error);
                        else
                            _tcs.SetResult(args.Error == null ? DialogResult : default(T));
                    };

            return _tcs.Task;
        }

        #endregion

        /// <summary>
        ///   Implicitly converts the current DialogOperationResult to type <see cref="Task" />
        /// </summary>
        /// <param name="operation"> The DialogOperationResult to be converted. </param>
        public static implicit operator Task(DialogOperationResult<T> operation)
        {
            return operation.AsTask();
        }

        /// <summary>
        ///   Implicitly converts the current DialogOperationResult to type <see cref="Task{T}" />
        /// </summary>
        /// <param name="operation"> The DialogOperationResult to be converted. </param>
        public static implicit operator Task<T>(DialogOperationResult<T> operation)
        {
            return operation.AsTask();
        }
    }
}