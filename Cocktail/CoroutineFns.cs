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
using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro;
using IdeaBlade.EntityModel;
using Action = System.Action;

namespace Cocktail
{
    /// <summary>
    /// A collection of extension methods providing Coroutine functionality.
    /// </summary>
    public static class CoroutineFns
    {
        /// <summary>
        /// Returns an implementation of <see cref = "IResult" /> that enables sequential execution of multiple results.
        /// </summary>
        /// <param name="results">The results to be executed sequentially</param>
        /// <returns>A result enabling sequential execution.</returns>
        public static IResult ToSequentialResult(this IEnumerable<IResult> results)
        {
            return new SequentialResult(results.GetEnumerator());
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult" /> that wraps a DevForce asynchronous function.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult AsOperationResult(this INotifyCompleted asyncOp)
        {
            if (asyncOp is OperationResult)
                return (OperationResult) asyncOp;
            return new OperationResult(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<object> AsOperationResult(this CoroutineOperation asyncOp)
        {
            return new CoroutineOperationResult<object>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<T> AsOperationResult<T>(this CoroutineOperation asyncOp)
        {
            return new CoroutineOperationResult<T>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<IEnumerable> AsOperationResult(this EntityQueryOperation asyncOp)
        {
            return new EntityQueryOperationResult(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<IEnumerable<T>> AsOperationResult<T>(this EntityQueryOperation asyncOp)
        {
            return new EntityQueryOperationResult<T>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<IEnumerable<T>> AsOperationResult<T>(this EntityQueryOperation<T> asyncOp)
        {
            return new EntityQueryOperationResult<T>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<IEnumerable> AsOperationResult(this EntityRefetchOperation asyncOp)
        {
            return new EntityRefetchOperationResult(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<SaveResult> AsOperationResult(this EntitySaveOperation asyncOp)
        {
            return new EntitySaveOperationResult(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<object> AsOperationResult(this EntityScalarQueryOperation asyncOp)
        {
            return new EntityScalarQueryOperationResult<object>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<T> AsOperationResult<T>(this EntityScalarQueryOperation asyncOp)
        {
            return new EntityScalarQueryOperationResult<T>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<T> AsOperationResult<T>(this EntityScalarQueryOperation<T> asyncOp)
        {
            return new EntityScalarQueryOperationResult<T>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<object> AsOperationResult(this InvokeServerMethodOperation asyncOp)
        {
            return new InvokeServerMethodOperationResult<object>(asyncOp);
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult&lt;T&gt;" /> that wraps a DevForce asynchronous function
        /// and provides access to the operation's result value.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult<T> AsOperationResult<T>(this InvokeServerMethodOperation asyncOp)
        {
            return new InvokeServerMethodOperationResult<T>(asyncOp);
        }

        /// <summary>
        /// Executes a result with optional callback upon completion.
        /// </summary>
        /// <param name="result">The result to execute</param>
        /// <param name="callback">Optional callback</param>
        public static void Execute(this IResult result, Action<ResultCompletionEventArgs> callback = null)
        {
            if (callback != null)
                result.Completed += (sender, args) => Caliburn.Micro.Execute.OnUIThread(() => callback(args));
            result.Execute(null);
        }

        /// <summary>
        /// Executes a result and calls a callback upon completion.
        /// </summary>
        /// <param name="result">The result to be executed.</param>
        /// <param name="callback">Callback to be called when complete</param>
        public static void OnComplete(this IResult result, Action<ResultCompletionEventArgs> callback)
        {
            if (callback == null) return;

            result.Execute(callback);
        }

        /// <summary>
        /// Executes a result and calls separate callbacks for success and failure.
        /// </summary>
        /// <param name="result">The result to be executed</param>
        /// <param name="onSuccess">Callback to be called when result completes without error</param>
        /// <param name="onFail">Callback to be called when result completes with error</param>
        public static void OnComplete(this IResult result, Action onSuccess, Action<Exception> onFail)
        {
            if (onSuccess == null && onFail == null) return;

            result.Execute(args =>
                               {
                                   if (args.Error == null && !args.WasCancelled && onSuccess != null)
                                       onSuccess();

                                   if (args.Error != null && onFail != null)
                                       onFail(args.Error);
                               });
        }

        /// <summary>Ensures that a Coroutine continues if the current operation encounters an error and
        /// prevents an unhandled exception from being thrown.</summary>
        /// <param name="operation">The operation, whose error should be marked as handled.</param>
        /// <typeparam name="T">The type of the operation.</typeparam>
        /// <returns>The current operation.</returns>
        public static T ContinueOnError<T>(this T operation) where T : INotifyCompleted
        {
            operation.WhenCompleted(args => args.IsErrorHandled = args.Error != null);
            return operation;
        }
    }
}