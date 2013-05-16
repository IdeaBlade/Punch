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

namespace Cocktail
{
    /// <summary>
    ///     Provides data for navigation methods that cannot cancel the navigation request.
    /// </summary>
    public class NavigationArgs
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="parameter">Parameter passed to the target view model.</param>
        public NavigationArgs(object parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        ///     Gets the parameter passed to the target view model.
        /// </summary>
        public object Parameter { get; private set; }
    }

    /// <summary>
    ///     Provides data for the <see cref="INavigationTarget.OnNavigatingFrom" /> callback that can be used to cancel a navigation request.
    /// </summary>
    public class NavigationCancelArgs
    {
        /// <summary>
        ///     Specifies whether a pending navigation should be canceled.
        /// </summary>
        public bool IsCanceled { get; private set; }

        /// <summary>
        ///     Cancel the current navigation request.
        /// </summary>
        public void Cancel()
        {
            IsCanceled = true;
        }
    }

    /// <summary>
    ///     An optional interface for a view model to add code that responds to navigation events.
    /// </summary>
    public interface INavigationTarget
    {
        /// <summary>
        ///     Invoked when the view model becomes the current active view model at the end of a navigation request.
        /// </summary>
        /// <param name="args">Data relating to the pending navigation request.</param>
        void OnNavigatedTo(NavigationArgs args);

        /// <summary>
        ///     Invoked immediately before the view model is deactivated and is no longer the active view model due to a navigation request.
        /// </summary>
        /// <param name="args">Data relating to the pending navigation request.</param>
        void OnNavigatingFrom(NavigationCancelArgs args);

        /// <summary>
        ///     Invoked immediatly after the view model is deactivated and is no longer the active view model due to a navigation request.
        /// </summary>
        /// <param name="args"></param>
        void OnNavigatedFrom(NavigationArgs args);
    }
}