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
using System.Threading;

namespace Cocktail
{
    /// <summary>
    ///   Encapsulates and abstracts an asynchronous paging operation.
    /// </summary>
    /// <typeparam name="T"> The type of objects being paged. </typeparam>
    public class PageOperationResult<T> : OperationResult<Page<T>>
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly OperationResult<Page<T>> _operation;

        internal PageOperationResult(OperationResult<Page<T>> operation, CancellationTokenSource cancellationTokenSource)
            : base(operation)
        {
            _operation = operation;
            _cancellationTokenSource = cancellationTokenSource;
        }

        /// <summary>
        ///   The result value of the operation.
        /// </summary>
        public override Page<T> Result
        {
            get { return _operation.Result; }
        }

        /// <summary>
        ///   Returns whether the paging operation can be cancelled.
        /// </summary>
        public bool CanCancel
        {
            get { return !_operation.IsCompleted; }
        }

        /// <summary>
        ///   Cancels the current paging operation.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current paging operation cannot be cancelled.</exception>
        public void Cancel()
        {
            if (!CanCancel)
                throw new InvalidOperationException(StringResources.CannotCancelCurrentOperation);

            _cancellationTokenSource.Cancel();
        }
    }
}