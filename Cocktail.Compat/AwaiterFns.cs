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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///   A collection of extension methods to get awaiters for Cocktail and DevForce asynchronous operations.
    /// </summary>
    public static class AwaiterFns
    {
        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter GetAwaiter(this IAwaitable operation)
        {
            return operation.AsTask().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<T> GetAwaiter<T>(this IAwaitable<T> operation)
        {
            return operation.AsTask().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<object> GetAwaiter(this CoroutineOperation operation)
        {
            return operation.AsOperationResult<object>().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<IEnumerable> GetAwaiter(this EntityQueryOperation operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<IEnumerable<T>> GetAwaiter<T>(this EntityQueryOperation<T> operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<IEnumerable> GetAwaiter(this EntityRefetchOperation operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<SaveResult> GetAwaiter(this EntitySaveOperation operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<object> GetAwaiter(this EntityScalarQueryOperation operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<T> GetAwaiter<T>(this EntityScalarQueryOperation<T> operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter<object> GetAwaiter(this InvokeServerMethodOperation operation)
        {
            return operation.AsOperationResult<object>().GetAwaiter();
        }

        /// <summary>
        ///   Gets an awaiter used to await this operation.
        /// </summary>
        /// <param name="operation"> The operation to await. </param>
        public static TaskAwaiter GetAwaiter(this LoginOperation operation)
        {
            return operation.AsOperationResult().GetAwaiter();
        }
    }
}