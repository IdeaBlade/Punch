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

namespace Cocktail
{
    /// <summary>
    ///   Marks an awaitable object.
    /// </summary>
    public interface IAwaitable
    {
        /// <summary>
        ///   Converts the current object to <see cref="Task" /> .
        /// </summary>
        Task AsTask();
    }

    /// <summary>
    ///   Marks an awaitable object with a result value.
    /// </summary>
    /// <typeparam name="T"> The type of the result value. </typeparam>
    public interface IAwaitable<T> : IAwaitable
    {
        /// <summary>
        ///   Converts the current object to <see cref="Task{T}" /> .
        /// </summary>
        /// <returns> The type of the result value. </returns>
        new Task<T> AsTask();
    }
}