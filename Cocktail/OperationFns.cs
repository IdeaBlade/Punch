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
using System.Linq;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>A collection of DevForce asynchronous operation extension methods.</summary>
    public static class OperationFns
    {
        /// <summary>Extension method to process the result of an asynchronous query operation.</summary>
        /// <param name="source">The EntityQueryOperation returned from an asynchronous query.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous query was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous query failed.</param>
        /// <returns>Returns the EntityQueryOperation passed to the method's source parameter.</returns>
        public static EntityQueryOperation OnComplete(this EntityQueryOperation source,
                                                      Action<IEnumerable> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.Results);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous query operation.</summary>
        /// <param name="source">The EntityQueryOperation returned from an asynchronous query.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous query was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous query failed.</param>
        /// <remarks>This overload automatically attempts to cast the results from IEnumerable to IEnumerable&lt;T&gt;.</remarks>
        /// <returns>Returns the EntityQueryOperation passed to the method's source parameter.</returns>
        public static EntityQueryOperation OnComplete<T>(this EntityQueryOperation source,
                                                      Action<IEnumerable<T>> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.Results.Cast<T>());
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous query operation.</summary>
        /// <param name="source">The EntityQueryOperation returned from an asynchronous query.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous query was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous query failed.</param>
        /// <remarks>This overload ignores the query results. It's typically used when calling a stored procedure, which doesn't return anything.</remarks>
        /// <returns>Returns the EntityQueryOperation passed to the method's source parameter.</returns>
        public static EntityQueryOperation OnComplete(this EntityQueryOperation source,
                                                      Action onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess();
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous query operation.</summary>
        /// <typeparam name="T">The type of entity queried.</typeparam>
        /// <param name="source">The EntityQueryOperation returned from an asynchronous query.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous query was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous query failed.</param>
        /// <returns>Returns the EntityQueryOperation passed to the method's source parameter.</returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// public OperationResult GetCustomers(Action&lt;IEnumerable&lt;Customer&gt;&gt; onSuccess = null,
        ///                                      Action&lt;Exception&gt; onFail = null)
        /// {
        ///     var op = Manager.Customers.ExecuteAsync();
        ///     return op.OnComplete(onSuccess, onFail).AsOperationResult();
        /// }</code>
        /// </example>
        public static EntityQueryOperation<T> OnComplete<T>(this EntityQueryOperation<T> source,
                                                            Action<IEnumerable<T>> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.Results);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous scalar query operation.</summary>
        /// <param name="source">The EntityScalarQueryOperation returned from an asynchronous query.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous query was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous query failed.</param>
        /// <returns>Returns the EntityScalarQueryOperation passed to the method's source parameter.</returns>
        public static EntityScalarQueryOperation OnComplete(this EntityScalarQueryOperation source,
                                                            Action<object> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.Result);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous scalar query operation.</summary>
        /// <typeparam name="T">The type of entity queried.</typeparam>
        /// <param name="source">The EntityScalarQueryOperation returned from an asynchronous query.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous query was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous query failed.</param>
        /// <returns>Returns the EntityScalarQueryOperation passed to the method's source parameter.</returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// public OperationResult GetCustomerCount(Action&lt;int&gt; onSuccess = null,
        ///                                      Action&lt;Exception&gt; onFail = null)
        /// {
        ///     var op = Manager.Customers.AsScalarAsync().Count();
        ///     return op.OnComplete(onSuccess, onFail).AsOperationResult();
        /// }</code>
        /// </example>
        public static EntityScalarQueryOperation<T> OnComplete<T>(this EntityScalarQueryOperation<T> source,
                                                                  Action<T> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.Result);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of a Coroutine operation with result value.</summary>
        /// <param name="source">The CoroutineOperation returned from Coroutine.Start()</param>
        /// <param name="onSuccess">A callback to be called if the Coroutine was successful.
        ///  The result object is first cast to the expected type.</param>
        /// <param name="onFail">A callback to be called if the Coroutine failed.</param>
        /// <returns>Returns the CoroutineOperation passed to the method's source parameter.</returns>        
        public static CoroutineOperation OnComplete<T>(this CoroutineOperation source,
                                                       Action<T> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess((T)args.Result);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of a Coroutine operation without result value.</summary>
        /// <param name="source">The CoroutineOperation returned from Coroutine.Start()</param>
        /// <param name="onSuccess">A callback to be called if the Coroutine was successful.</param>
        /// <param name="onFail">A callback to be called if the Coroutine failed.</param>
        /// <returns>Returns the CoroutineOperation passed to the method's source parameter.</returns>        
        public static CoroutineOperation OnComplete(this CoroutineOperation source,
                                                       Action onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess();
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous save operation.</summary>
        /// <param name="source">The EntitySaveOperation returned from an asynchronous save.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous save was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous save failed.</param>
        /// <returns>Returns the EntitySaveOperation passed to the method's source parameter.</returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// public OperationResult Save(Action onSuccess = null, Action&lt;Exception&gt; onFail = null)
        /// {
        ///     EntitySaveOperation op = Manager.SaveChangesAsync();
        ///     return op.OnComplete(onSuccess, onFail).AsOperationResult();
        /// }</code>
        /// </example>
        public static EntitySaveOperation OnComplete(this EntitySaveOperation source,
                                                     Action onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess();
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of an asynchronous save operation.</summary>
        /// <param name="source">The EntitySaveOperation returned from an asynchronous save.</param>
        /// <param name="onSuccess">A callback to be called if the asynchronous save was successful.</param>
        /// <param name="onFail">A callback to be called if the asynchronous save failed.</param>
        /// <returns>Returns the EntitySaveOperation passed to the method's source parameter.</returns>
        /// <example>
        /// 	<code title="Example" description="" lang="CS">
        /// public OperationResult&lt;SaveResult&gt; Save(Action&lt;SaveResult&gt; onSuccess = null, Action&lt;Exception&gt; onFail = null)
        /// {
        ///     EntitySaveOperation op = Manager.SaveChangesAsync();
        ///     return op.OnComplete(onSuccess, onFail).AsOperationResult();
        /// }</code>
        /// </example>
        public static EntitySaveOperation OnComplete(this EntitySaveOperation source,
                                                     Action<SaveResult> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.SaveResult);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of a server method operation with result value.</summary>
        /// <param name="source">The InvokeServerMethodOperation returned from InvokeServerMethodAsync()</param>
        /// <param name="onSuccess">A callback to be called if the server method was successful.
        ///  The result object is first cast to the expected type.</param>
        /// <param name="onFail">A callback to be called if the server method failed.</param>
        /// <returns>Returns the InvokeServerMethodOperation passed to the method's source parameter.</returns>        
        public static InvokeServerMethodOperation OnComplete<T>(this InvokeServerMethodOperation source,
                                                                Action<T> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess((T)args.Result);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of a server method operation without result value.</summary>
        /// <param name="source">The InvokeServerMethodOperation returned from InvokeServerMethodAsync()</param>
        /// <param name="onSuccess">A callback to be called if the server method was successful.</param>
        /// <param name="onFail">A callback to be called if the server method failed.</param>
        /// <returns>Returns the InvokeServerMethodOperation passed to the method's source parameter.</returns>       
        public static InvokeServerMethodOperation OnComplete(this InvokeServerMethodOperation source,
                                                             Action onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess();
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>Extension method to process the result of a refetch operation.</summary>
        /// <param name="source">The EntityRefetchOperation returned from an asynchronous refetch operation."/></param>
        /// <param name="onSuccess">A callback to be called if the refetch was successful.</param>
        /// <param name="onFail">A callback to be called if the refetch failed.</param>
        /// <returns>Returns the EntityRefetchOperation passed to the method's source parameter.</returns>       
        public static EntityRefetchOperation OnComplete(this EntityRefetchOperation source, 
            Action<IEnumerable> onSuccess, Action<Exception> onFail)
        {
            source.Completed += (s, args) =>
            {
                if (args.CompletedSuccessfully)
                {
                    if (onSuccess != null) onSuccess(args.Results);
                }

                if (args.HasError && !args.IsErrorHandled && onFail != null)
                {
                    args.MarkErrorAsHandled();
                    onFail(args.Error);
                }
            };
            return source;
        }

        /// <summary>
        /// Returns an implementation of <see cref = "OperationResult" /> that wraps a DevForce asynchronous function.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>OperationResult encapsulating the provided DevForce asynchronous operation.</returns>
        public static OperationResult AsOperationResult(this INotifyCompleted asyncOp)
        {
            if (asyncOp is OperationResult)
                return (OperationResult)asyncOp;
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

        /// <summary>Ensures that a Coroutine continues if the current operation encounters an error and
        /// prevents an unhandled exception from being thrown.</summary>
        /// <param name="operation">The operation for which the error should be marked as handled.</param>
        /// <typeparam name="T">The type of the operation.</typeparam>
        /// <returns>The current operation.</returns>
        public static T ContinueOnError<T>(this T operation) where T : INotifyCompleted
        {
            operation.WhenCompleted(args => args.IsErrorHandled = args.Error != null);
            return operation;
        }

        /// <summary>
        /// Creates a continuation that executes when the target operation completes.
        /// </summary>
        /// <param name="operation">The target operation.</param>
        /// <param name="continuationAction">An action to run when the operation completes. When run, the delegate will be passed the completed operation as an argument. If the operation completed with an error, the error will be marked as handled.</param>
        /// <typeparam name="T">The type of the target operation.</typeparam>
        /// <returns>The completed target operation.</returns>
        /// <exception cref="ArgumentNullException">The continuationAction argument is null.</exception>
        public static T ContinueWith<T>(this T operation, Action<T> continuationAction) where T : INotifyCompleted
        {
            if (continuationAction == null) throw new ArgumentNullException("continuationAction");

            operation.WhenCompleted(
                args =>
                    {
                        if (args.Error != null)
                            args.IsErrorHandled = true;

                        continuationAction(operation);
                    });
            return operation;
        }
    }
}