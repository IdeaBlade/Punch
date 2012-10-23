// ====================================================================================================================
//  Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//  WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//  OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//  OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//  USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//  http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using IdeaBlade.Core;
using IdeaBlade.Core.Composition;

namespace Cocktail
{
    /// <summary>
    /// Interface to be implemented when custom <see cref="ConnectionOptions"/> are used.
    /// </summary>
    [InterfaceExport(typeof(IConnectionOptionsResolver))]
    public interface IConnectionOptionsResolver : IHideObjectMembers
    {
        /// <summary>
        /// Called by Cocktail to retrieve the ConnectionOptions having the specified name.
        /// </summary>
        /// <param name="name">The name of the ConnectonOptions to be retrieved.</param>
        /// <returns></returns>
        ConnectionOptions GetConnectionOptions(string name);
    }
}