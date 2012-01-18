//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>Provides extension methods for the DevForce <see cref="BaseOperation"/> implementations.</summary>
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
    }
}