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
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Cocktail
{
    public partial class NavigationService
    {
        private readonly IConductActiveItem _conductor;

        /// <summary>
        ///   Initializes a new NavigationService.
        /// </summary>
        /// <param name="conductor"> The underlying screen conductor used to activate navigation targets. </param>
        public NavigationService(IConductActiveItem conductor)
        {
            _conductor = conductor;
        }

        #region INavigationService Members

        /// <summary>
        ///   Returns the current active ViewModel or null.
        /// </summary>
        public object ActiveViewModel
        {
            get { return _conductor.ActiveItem; }
        }

        /// <summary>
        ///   Asynchronously navigates to an instance of the provided ViewModel type. The navigation will be cancelled if 
        ///   the current active ViewModel cannot be closed or the target type is not authorized.
        /// </summary>
        /// <param name="viewModelType"> The target ViewModel type. </param>
        /// <param name="prepare"> An action to initialize the target ViewModel before it is activated. </param>
        /// <returns> A <see cref="Task" /> to await completion. </returns>
        public async Task NavigateToAsync(Type viewModelType, Func<object, Task> prepare)
        {
            if (viewModelType == null) throw new ArgumentNullException("viewModelType");
            if (prepare == null) throw new ArgumentNullException("prepare");

            var canClose = await CanCloseAsync();
            if (!canClose)
                throw new TaskCanceledException("The ActiveViewModel cannot be closed in the current state.");

            var targetAuthorized = await AuthorizeTargetAsync(viewModelType);
            if (!targetAuthorized)
                throw new TaskCanceledException("The target type is not authorized");

            var target = Composition.GetInstance(viewModelType, null);
            await prepare(target);

            if (!ReferenceEquals(ActiveViewModel, target))
                _conductor.ActivateItem(target);
        }

        #endregion
    }
}