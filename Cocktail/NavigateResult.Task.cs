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
    public partial class NavigateResult<T> : IAwaitable
    {
        private TaskCompletionSource<bool> _tcs;

        #region IAwaitable Members

        /// <summary>
        ///   Returns a Task&lt;T&gt; for the current NavigateResult.
        /// </summary>
        public Task AsTask()
        {
            if (_tcs != null) return _tcs.Task;

            _tcs = new TaskCompletionSource<bool>();
            ((IResult) this).Completed +=
                (sender, args) =>
                    {
                        if (args.WasCancelled)
                            _tcs.SetCanceled();
                        else if (args.Error != null)
                            _tcs.SetException(args.Error);
                        else
                            _tcs.SetResult(true);
                    };

            if (_status == Status.WaitingToRun)
                Go();

            return _tcs.Task;
        }

        #endregion

        /// <summary>
        ///   Implicitly converts the current NavigateResult to type <see cref="Task" />
        /// </summary>
        /// <param name="operation"> The NavigateResult to be converted. </param>
        public static implicit operator Task(NavigateResult<T> operation)
        {
            return operation.AsTask();
        }
    }
}