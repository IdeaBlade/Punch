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
using Cocktail.Contrib.UnitOfWork;
using DomainModel;
using DomainServices.Factories;
using DomainServices.Repositories;

namespace DomainServices
{
    [Export(typeof (IStaffingResourceUnitOfWork)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceUnitOfWork : UnitOfWorkCore, IStaffingResourceUnitOfWork
    {
        [ImportingConstructor]
        public StaffingResourceUnitOfWork(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider,
            [Import(AllowDefault = true)] IPreLoader preLoader = null)
            : base(entityManagerProvider, preLoader)
        {
            StaffingResourceFactory = new StaffingResourceFactory(entityManagerProvider, AddressTypes, PhoneNumberTypes);
            RateTypes = new PreLoadRepository<RateType>(entityManagerProvider, preLoader);
            StaffingResources = new StaffingResourceRepository(entityManagerProvider);
        }

        #region IStaffingResourceUnitOfWork Members

        public IStaffingResourceFactory StaffingResourceFactory { get; private set; }

        public IRepository<RateType> RateTypes { get; private set; }
        public IRepository<StaffingResource> StaffingResources { get; private set; }

        #endregion
    }
}