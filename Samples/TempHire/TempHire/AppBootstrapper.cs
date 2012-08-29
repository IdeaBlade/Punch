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

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using Cocktail;
using Common;
using Common.Workspace;
using DomainModel;
using TempHire.ViewModels;
using TempHire.ViewModels.StaffingResource;

namespace TempHire
{
    public class AppBootstrapper : BootstrapperBase<ShellViewModel>
    {
        protected override void PrepareCompositionContainer(CompositionBatch batch)
        {
            base.PrepareCompositionContainer(batch);

            // Configure workspaces
            batch.AddExportedValue<IWorkspace>(
                new Workspace("Home", true, 0, typeof (HomeViewModel)));
            batch.AddExportedValue<IWorkspace>(
                new Workspace("Resource Management", false, 10, typeof (ResourceMgtViewModel)));
        }

#if FAKESTORE || DEMO
        [Import] public IEntityManagerProvider<TempHireEntities> EntityManagerProvider;

        protected override Task StartRuntimeAsync()
        {
            //TODO: There's currently a bug in DevForce 2012 that prevents asynchronous initialization with the local fake backing store.
#if SILVERLIGHT
            return EntityManagerProvider.InitializeFakeBackingStoreAsync();
#else
            EntityManagerProvider.InitializeFakeBackingStore();
            return Task.FromResult(true);
#endif
        }
#endif
    }
}