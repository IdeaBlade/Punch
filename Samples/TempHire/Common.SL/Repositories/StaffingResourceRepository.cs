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

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;

namespace Common.Repositories
{
    [Export(typeof(IStaffingResourceRepository))]
    public class StaffingResourceRepository : RepositoryBase<TempHireEntities>, IStaffingResourceRepository
    {
        [ImportingConstructor]
        public StaffingResourceRepository(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider,
            [Import(AllowDefault = true)] ILookupRepository lookupRepository = null)
            : base(entityManagerProvider, lookupRepository as RepositoryBase<TempHireEntities>)
        {
        }

        #region IStaffingResourceRepository Members

        public OperationResult<StaffingResource> CreateStaffingResourceAsync(
            string firstName, string middleName, string lastName, Action<StaffingResource> onSuccess = null,
            Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => CreateResourceCore(firstName, middleName, lastName),
                                   op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult<StaffingResource>();
        }

        public OperationResult<IEnumerable<StaffingResource>> GetAllStaffingResourcesAsync(
            Action<IEnumerable<StaffingResource>> onSuccess = null, Action<Exception> onFail = null)
        {
            IEntityQuery<StaffingResource> query =
                Manager.StaffingResources.OrderBy(r => r.LastName).ThenBy(r => r.FirstName);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult<StaffingResource> GetStaffingResourceAsync(
            Guid staffingResourceId, Action<StaffingResource> onSuccess = null, Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => GetStaffingResourceCore(staffingResourceId),
                                   op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult<StaffingResource>();
        }

        public OperationResult<IEnumerable<AddressType>> GetAddressTypesAsync(
            Action<IEnumerable<AddressType>> onSuccess = null,
            Action<Exception> onFail = null)
        {
            IEntityQuery<AddressType> query = Manager.AddressTypes.OrderBy(t => t.Name).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult<IEnumerable<PhoneNumberType>> GetPhoneTypesAsync(
            Action<IEnumerable<PhoneNumberType>> onSuccess = null, Action<Exception> onFail = null)
        {
            IEntityQuery<PhoneNumberType> query =
                Manager.PhoneNumberTypes.OrderBy(t => t.Name).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult<IEnumerable<RateType>> GetRateTypesAsync(
            Action<IEnumerable<RateType>> onSuccess = null, Action<Exception> onFail = null)
        {
            IEntityQuery<RateType> query = Manager.RateTypes.OrderBy(t => t.Sequence).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult<IEnumerable<StaffingResourceListItem>> FindStaffingResourcesAsync(
            string searchText, string orderBy, Action<IEnumerable<StaffingResourceListItem>> onSuccess = null,
            Action<Exception> onFail = null)
        {
            IEntityQuery<StaffingResource> baseQuery = Manager.StaffingResources;

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

            IEntityQuery<StaffingResourceListItem> query =
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

            if (!string.IsNullOrWhiteSpace(orderBy))
                query = query.OrderBySelector(new SortSelector(orderBy));

            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult DeleteStaffingResourceAsync(Guid staffingResourceId, Action onSuccess = null,
                                                           Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => DeleteResourceCore(staffingResourceId), op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult();
        }

        public OperationResult<IEnumerable<State>> GetStatesAsync(Action<IEnumerable<State>> onSuccess = null,
                                                                  Action<Exception> onFail = null)
        {
            IEntityQuery<State> query = Manager.States.OrderBy(s => s.Name).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        #endregion

        private IEnumerable<INotifyCompleted> GetStaffingResourceCore(Guid staffingResourceId)
        {
            // Execute as IEntityQuery instead of IEntityScalarQuery in order to be cacheable. 
            // IEntityScalarQuery is always satisfied from the data source.
            IEntityQuery<StaffingResource> query = Manager.StaffingResources
                .Where(r => r.Id == staffingResourceId)
                .Include(r => r.Addresses)
                .Include(r => r.PhoneNumbers);

            EntityQueryOperation<StaffingResource> queryOperation;
            yield return queryOperation = query.ExecuteAsync();

            yield return Coroutine.Return(queryOperation.Results.First());
        }

        private IEnumerable<INotifyCompleted> DeleteResourceCore(Guid staffingResourceId)
        {
            StaffingResource staffingResource = null;
            yield return GetStaffingResourceAsync(staffingResourceId, result => staffingResource = result);

            EntityAspect.Wrap(staffingResource).Delete();
            yield return Manager.SaveChangesAsync();
        }

        private IEnumerable<INotifyCompleted> CreateResourceCore(string firstName, string middleName, string lastName)
        {
            StaffingResource staffingResource = StaffingResource.Create();
            staffingResource.FirstName = firstName;
            staffingResource.MiddleName = middleName;
            staffingResource.LastName = lastName;
            Manager.AddEntity(staffingResource);

            AddressType addressType = null;
            yield return Manager.AddressTypes.Where(t => t.Default).With(BaseDataQueryStrategy)
                .ExecuteAsync(op => op.OnComplete(result => addressType = result.First(), null));
            staffingResource.AddAddress(addressType);
            staffingResource.PrimaryAddress = staffingResource.Addresses.First();

            PhoneNumberType phoneType = null;
            yield return Manager.PhoneNumberTypes.Where(t => t.Default).With(BaseDataQueryStrategy)
                .ExecuteAsync(op => op.OnComplete(result => phoneType = result.First(), null));
            staffingResource.AddPhoneNumber(phoneType);
            staffingResource.PrimaryPhoneNumber = staffingResource.PhoneNumbers.First();

            yield return Coroutine.Return(staffingResource);
        }
    }
}