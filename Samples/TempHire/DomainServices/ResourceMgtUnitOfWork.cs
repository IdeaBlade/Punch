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
using Cocktail;
using DomainModel;
using DomainServices.Factories;
using DomainServices.Repositories;
using DomainServices.Services;

namespace DomainServices
{
    [Export(typeof(IResourceMgtUnitOfWork)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceMgtUnitOfWork : UnitOfWork, IResourceMgtUnitOfWork
    {
        [ImportingConstructor]
        public ResourceMgtUnitOfWork(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider,
            [Import(AllowDefault = true)] IGlobalCache globalCache = null)
            : base(entityManagerProvider)
        {
            AddressTypes = new GlobalCacheRepository<AddressType>(entityManagerProvider, globalCache);
            States = new GlobalCacheRepository<State>(entityManagerProvider, globalCache);
            PhoneNumberTypes = new GlobalCacheRepository<PhoneNumberType>(entityManagerProvider, globalCache);
            RateTypes = new GlobalCacheRepository<RateType>(entityManagerProvider, globalCache);
            StaffingResourceFactory = new StaffingResourceFactory(entityManagerProvider, AddressTypes, PhoneNumberTypes);
            StaffingResources = new StaffingResourceRepository(entityManagerProvider);
            Search = new StaffingResourceSearchService(StaffingResources);
        }

        #region IResourceMgtUnitOfWork Members

        public IFactory<StaffingResource> StaffingResourceFactory { get; private set; }

        public IRepository<AddressType> AddressTypes { get; private set; }
        public IRepository<State> States { get; private set; }
        public IRepository<PhoneNumberType> PhoneNumberTypes { get; private set; }
        public IRepository<RateType> RateTypes { get; private set; }
        public IRepository<StaffingResource> StaffingResources { get; private set; }

        public IStaffingResourceSearchService Search { get; private set; }

        #endregion
    }
}