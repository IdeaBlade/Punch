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
using System.Collections.Generic;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// A collection of <see cref="IResult"/> extension methods.
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
    }
}