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
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>
    ///   ViewModel implementing the Development Harness. Specify this ViewModel as the TRootModel in the Application Bootstrapper to create a Development Harness
    /// </summary>
    /// <example>
    ///   <code title="Development Harness Bootstrapper"
    ///     description="Demonstrates how to specify the HarnessView model in the Application Bootstrapper to create a Development Harness."
    ///     lang="CS">public class AppBootstrapper : FrameworkBootstrapper&lt;HarnessViewModel&gt;
    ///     {
    ///     // Additional code
    ///     }</code>
    /// </example>
    [Export]
    public class HarnessViewModel : Conductor<object>
    {
        private readonly Dictionary<string, object> _viewModels;
        private string _activeName;

        /// <summary>
        ///   Initializes a new instance.
        /// </summary>
        /// <param name="viewModels"> The list of discovered ViewModels injected through MEF. </param>
        [ImportingConstructor]
        public HarnessViewModel([ImportMany] IEnumerable<IDiscoverableViewModel> viewModels)
        {
            _viewModels = new Dictionary<string, object>();
            viewModels.ForEach(vm => _viewModels.Add(vm.GetType().Name, vm));

            EventFns.Subscribe(this);
        }

        /// <summary>
        ///   Bindable collection exposing the names of all discovered ViewModels.
        /// </summary>
        public BindableCollection<string> Names
        {
            get { return new BindableCollection<string>(_viewModels.Keys.OrderBy(k => k)); }
        }

        /// <summary>
        ///   Returns the name of the current active view model.
        /// </summary>
        public string ActiveName
        {
            get { return _activeName; }
            set
            {
                _activeName = value;
                NotifyOfPropertyChange(() => ActiveName);
            }
        }

        /// <summary>
        ///   Activates the ViewModel with the given name.
        /// </summary>
        public void ActivateViewModel(string name)
        {
            object viewModel;
            if (_viewModels.TryGetValue(name, out viewModel))
            {
                var harnessAware = viewModel as IHarnessAware;
                if (harnessAware != null)
                    harnessAware.Setup();

                ActivateItem(viewModel);
                ActiveName = name;
            }
        }
    }
}