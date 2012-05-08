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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
    [Export(typeof(IStaffingResourceSearchRepository)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffingResourceSearchRepository : IStaffingResourceSearchRepository
    {
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;

        [ImportingConstructor]
        public StaffingResourceSearchRepository(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        private TempHireEntities EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IStaffingResourceSearchRepository Members

        public OperationResult<IEnumerable<StaffingResourceListItem>> FindStaffingResourcesAsync(
            string searchText,
            Func<IQueryable<StaffingResourceListItem>, IOrderedQueryable<StaffingResourceListItem>> orderBy = null,
            Action<IEnumerable<StaffingResourceListItem>> onSuccess = null, Action<Exception> onFail = null)
        {
            IEntityQuery<StaffingResource> baseQuery = EntityManager.StaffingResources;

            if (!string.IsNullOrWhiteSpace(searchText))
                baseQuery = baseQuery.Where(resource => resource.FirstName.Contains(searchText) ||
                                                        resource.MiddleName.Contains(searchText) ||
                                                        resource.LastName.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).Address1.
                                                            Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).Address2.
                                                            Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).City.Contains(
                                                            searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).Zipcode.
                                                            Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).State.Name.
                                                            Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).State.
                                                            ShortName.Contains(searchText) ||
                                                        resource.PhoneNumbers.FirstOrDefault(p => p.Primary).AreaCode.
                                                            Contains(searchText) ||
                                                        resource.PhoneNumbers.FirstOrDefault(p => p.Primary).Number.
                                                            Contains(searchText));

            var query =
                baseQuery.Select(resource => new StaffingResourceListItem
                                                 {
                                                     Id = resource.Id,
                                                     FirstName = resource.FirstName,
                                                     MiddleName = resource.MiddleName,
                                                     LastName = resource.LastName,
                                                     Address1 =
                                                         resource.Addresses.FirstOrDefault(a => a.Primary).Address1,
                                                     Address2 =
                                                         resource.Addresses.FirstOrDefault(a => a.Primary).Address2,
                                                     City = resource.Addresses.FirstOrDefault(a => a.Primary).City,
                                                     State =
                                                         resource.Addresses.FirstOrDefault(a => a.Primary).State.
                                                         ShortName,
                                                     Zipcode = resource.Addresses.FirstOrDefault(a => a.Primary).Zipcode,
                                                     AreaCode =
                                                         resource.PhoneNumbers.FirstOrDefault(p => p.Primary).AreaCode,
                                                     Number =
                                                         resource.PhoneNumbers.FirstOrDefault(p => p.Primary).Number
                                                 });

            if (orderBy != null)
                query = (IEntityQuery<StaffingResourceListItem>)orderBy(query);

            return query.ExecuteAsync(op => op.OnComplete(onSuccess, onFail)).AsOperationResult();
        }

        #endregion
    }
}