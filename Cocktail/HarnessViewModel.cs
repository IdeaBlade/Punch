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
using System.Linq;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    ///   ViewModel implementing the Development Harness. Specify this ViewModel as the root in the Application constructor to create a Development Harness
    /// </summary>
    public partial class HarnessViewModel
    {
        private readonly Dictionary<string, object> _viewModels;
        private string _activeName;

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
    }
}