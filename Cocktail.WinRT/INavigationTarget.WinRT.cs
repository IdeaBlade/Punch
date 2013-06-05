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

using System.Collections.Generic;

namespace Cocktail
{
    public partial interface INavigationTarget
    {
        /// <summary>
        /// Uniquely identifies the current view model instance. When implementing INavigationTarget, do not modify PageKey. PageKey is automatically maintained by the <see cref="INavigator"/> implementation.
        /// </summary>
        string PageKey { get; set; }

        /// <summary>
        /// Invoked to populate a view model with content passed during navigation. Any saved state
        /// is also provided when recreating a view model from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="INavigator.NavigateToAsync(System.Type, object)"/>.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this view model during an earlier
        /// session. This will be null the first time a view model is visited.</param>
        /// <param name="sharedState">A dictionary of shared state preserved by any view model during an earlier session.</param>
        void LoadState(object navigationParameter, Dictionary<string, object> pageState, Dictionary<string, object> sharedState);

        /// <summary>
        /// Preserves state associated with this view model in case the application is suspended or the
        /// view model is discarded from the navigation cache.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        /// <param name="sharedState">A dictionary to be populated with shared state. The dictionary may already contain state that can be overwritten.</param>
        void SaveState(Dictionary<string, object> pageState, Dictionary<string, object> sharedState);
    }
}