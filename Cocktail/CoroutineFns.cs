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
        public static IResult ToSequential(this IEnumerable<IResult> results)
        {
            return new SequentialResult(results.GetEnumerator());
        }

        /// <summary>
        /// Returns an implemementation of <see cref = "AsyncOperation" /> that wraps a DevForce asynchronous function.
        /// </summary>
        /// <param name="asyncOp">DevForce asynchronous operation.</param>
        /// <returns>A result enabling the calling of the DevForce asynchronous function.</returns>
        public static AsyncOperation AsOperationResult(this INotifyCompleted asyncOp)
        {
            return new AsyncOperation(asyncOp);
        }

        /// <summary>
        /// Executes a result with optional callback upon completion.
        /// </summary>
        /// <param name="result">The result to execute</param>
        /// <param name="callback">Optional callback</param>
        public static void Execute(this IResult result, Action<ResultCompletionEventArgs> callback = null)
        {
            if (callback != null)
                result.Completed += (sender, args) => callback(args);
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
    }
}