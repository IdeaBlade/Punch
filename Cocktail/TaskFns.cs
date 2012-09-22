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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cocktail
{
    /// <summary>
    ///   A set of cross-platform static and extension methods that operate on <see cref="Task" /> and <see cref="Task{T}" />
    /// </summary>
    public static class TaskFns
    {
        /// <summary>
        ///   Creates a <see cref="Task{T}" /> that's completed successfully with the specified result.
        /// </summary>
        /// <param name="resultValue"> The result value to store in the completed task. </param>
        public static Task<T> FromResult<T>(T resultValue)
        {
#if SILVERLIGHT
            return TaskEx.FromResult(resultValue);
#else
            return Task.FromResult(resultValue);
#endif
        }

        /// <summary>
        ///   Creates a <see cref="Task{T}" /> from a callback action that completes when the callback is called.
        /// </summary>
        /// <param name="action"> The callback action. </param>
        /// <typeparam name="T"> The type of the callback result. </typeparam>
        public static Task<T> FromCallbackAction<T>(Action<Action<T>> action)
        {
            var tcs = new TaskCompletionSource<T>();
            try
            {
                action(tcs.SetResult);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }

            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that will complete when all of the provided collection of Tasks have completed
        /// </summary>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task WhenAll(params Task[] tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the provided collection of Tasks have completed
        /// </summary>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the provided collection of Tasks have completed
        /// </summary>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task<T[]> WhenAll<T>(params Task<T>[] tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the provided collection of Tasks have completed
        /// </summary>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        public static Task<T[]> WhenAll<T>(IEnumerable<Task<T>> tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <returns>A task that represents the completion of one of the supplied tasks. The return Task's Result is the task that completed.</returns>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <returns>A task that represents the completion of one of the supplied tasks. The return Task's Result is the task that completed.</returns>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <returns>A task that represents the completion of one of the supplied tasks. The return Task's Result is the task that completed.</returns>
        public static Task<Task<T>> WhenAny<T>(params Task<T>[] tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <returns>A task that represents the completion of one of the supplied tasks. The return Task's Result is the task that completed.</returns>
        public static Task<Task<T>> WhenAny<T>(IEnumerable<Task<T>> tasks)
        {
#if SILVERLIGHT
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }
    }
}