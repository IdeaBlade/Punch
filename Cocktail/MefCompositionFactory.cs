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

#if !WinRT
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail
{
    /// <summary>
    /// A factory that creates new instances that provide the specified MEF export.
    /// </summary>
    public class MefCompositionFactory<T> : ICompositionFactory<T>
    {
        internal MefCompositionFactory()
        {
        }

        /// <summary>
        /// Internal use.
        /// </summary>
        [Import]
        public ExportFactory<T> ExportFactory { get; set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public T NewInstance()
        {
            return ExportFactory.CreateExport().Value;
        }
    }
}
