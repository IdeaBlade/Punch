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
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainServices.Repositories;
using IdeaBlade.EntityModel;

namespace TempHire
{
    [Export(typeof (IGlobalCache)), PartCreationPolicy(CreationPolicy.Shared)]
    public class GlobalCache : IGlobalCache
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        [ImportingConstructor]
        public GlobalCache(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        private TempHireEntities EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IGlobalCache Members

        public Task LoadAsync()
        {
            return TaskFns.WhenAll(
                EntityManager.PhoneNumberTypes.ExecuteAsync(),
                EntityManager.AddressTypes.ExecuteAsync(),
                EntityManager.RateTypes.ExecuteAsync(),
                EntityManager.States.ExecuteAsync());
        }

        public IEnumerable<T> Get<T>() where T : class
        {
            return EntityManager.FindEntities<T>(EntityState.Unchanged);
        }

        #endregion
    }
}