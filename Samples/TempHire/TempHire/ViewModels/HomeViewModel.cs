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

using System.ComponentModel.Composition;
using Caliburn.Micro;
using Cocktail;
using Common.Workspace;

namespace TempHire.ViewModels
{
    [Export]
    public class HomeViewModel : Screen, IWorkspace, IDiscoverableViewModel
    {
        public HomeViewModel()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            DisplayName = "Home";
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        #region IWorkspace Members

        public bool IsDefault
        {
            get { return true; }
        }

        public int Sequence
        {
            get { return 0; }
        }

        public IScreen Content
        {
            get { return this; }
        }

        #endregion
    }
}