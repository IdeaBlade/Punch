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

using Caliburn.Micro;
using Windows.UI.Xaml.Controls;

namespace Cocktail
{
    /// <summary>
    ///   UserControl implementing the development harness for Windows Store apps.
    /// </summary>
    public sealed partial class Harness : UserControl
    {
        /// <summary>
        ///   Initializes the development harness
        /// </summary>
        public Harness()
        {
            InitializeComponent();

            if (Execute.InDesignMode) 
                return;

            // Create and attach ViewModel
            var navigator = Composition.GetInstance<INavigator>();
            var viewModels = Composition.GetInstances<IDiscoverableViewModel>();
            DataContext = new HarnessViewModel(navigator, viewModels);
        }

        private void Button_Activate(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var vm = (HarnessViewModel) DataContext;
            var name = (string) ((Button) sender).DataContext;

            vm.ActivateViewModel(name);
        }
    }
}