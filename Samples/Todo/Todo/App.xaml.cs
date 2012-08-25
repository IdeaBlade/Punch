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

using System.Composition;
using System.Threading.Tasks;
using Cocktail;
using IdeaBlade.Core;
using Todo.Views;
using TodoServer;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Todo
{
    /// <summary>
    ///   Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : CocktailMefWindowsStoreApplication
    {
        /// <summary>
        ///   Initializes the singleton application object.  This is the first line of authored code
        ///   executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() : base(typeof(MainPage))
        {
            InitializeComponent();
        }

        [Import]
        public IEntityManagerProvider<TodoEntities> EntityManagerProvider { get; set; }

        protected override void StartRuntime()
        {
            base.StartRuntime();

            IdeaBladeConfig.Instance.ObjectServer.RemoteBaseUrl = "http://localhost";
            IdeaBladeConfig.Instance.ObjectServer.ServerPort = 55123;
            IdeaBladeConfig.Instance.ObjectServer.ServiceName = "EntityService.svc";
        }
    }
}