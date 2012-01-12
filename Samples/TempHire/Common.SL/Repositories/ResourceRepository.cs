using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using Action = System.Action;
using Coroutine = IdeaBlade.EntityModel.Coroutine;

namespace Common.Repositories
{
    [Export(typeof(IResourceRepository))]
    public class ResourceRepository : RepositoryBase<TempHireEntities>, IResourceRepository
    {
        [ImportingConstructor]
        public ResourceRepository(
            [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider<TempHireEntities>
                entityManagerProvider,
            [Import(AllowDefault = true)] ILookupRepository lookupRepository = null)
            : base(entityManagerProvider, lookupRepository as RepositoryBase<TempHireEntities>)
        {
        }

        #region IResourceRepository Members

        public OperationResult CreateResourceAsync(string firstName, string middleName, string lastName,
                                                    Action<Resource> onSuccess = null, Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => CreateResourceCore(firstName, middleName, lastName),
                                   op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult();
        }

        public OperationResult GetAllResourcesAsync(Action<IEnumerable<Resource>> onSuccess = null,
                                                     Action<Exception> onFail = null)
        {
            IEntityQuery<Resource> query = Manager.Resources.OrderBy(r => r.LastName).ThenBy(r => r.FirstName);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult GetResourceAsync(Guid resourceId, Action<Resource> onSuccess = null,
                                        Action<Exception> onFail = null)
        {
            // Execute as IEntityQuery instead of IEntityScalarQuery in order to be cacheable. 
            // IEntityScalarQuery is always satisfied from the Datasource.
            IEntityQuery<Resource> query = Manager.Resources
                .Where(r => r.Id == resourceId)
                .Include(r => r.Addresses)
                .Include(r => r.PhoneNumbers);

            return ExecuteQuery(query,
                                result => { if (onSuccess != null) onSuccess(result.First()); },
                                onFail);
        }

        public OperationResult GetAddressTypesAsync(Action<IEnumerable<AddressType>> onSuccess = null,
                                                     Action<Exception> onFail = null)
        {
            IEntityQuery<AddressType> query = Manager.AddressTypes.OrderBy(t => t.Name).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult GetPhoneTypesAsync(Action<IEnumerable<PhoneNumberType>> onSuccess = null,
                                                   Action<Exception> onFail = null)
        {
            IEntityQuery<PhoneNumberType> query =
                Manager.PhoneNumberTypes.OrderBy(t => t.Name).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult GetRateTypesAsync(Action<IEnumerable<RateType>> onSuccess = null,
                                                  Action<Exception> onFail = null)
        {
            IEntityQuery<RateType> query = Manager.RateTypes.OrderBy(t => t.Sequence).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult FindResourcesAsync(string searchText, string orderBy,
                                                   Action<IEnumerable<ResourceListItem>> onSuccess = null,
                                                   Action<Exception> onFail = null)
        {
            IEntityQuery<Resource> baseQuery = Manager.Resources;

            if (!string.IsNullOrWhiteSpace(searchText))
                baseQuery = baseQuery.Where(resource => resource.FirstName.Contains(searchText) ||
                                                        resource.MiddleName.Contains(searchText) ||
                                                        resource.LastName.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).Address1.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).Address2.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).City.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).Zipcode.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).State.Name.Contains(searchText) ||
                                                        resource.Addresses.FirstOrDefault(a => a.Primary).State.ShortName.Contains(searchText) ||
                                                        resource.PhoneNumbers.FirstOrDefault(p => p.Primary).AreaCode.Contains(searchText) ||
                                                        resource.PhoneNumbers.FirstOrDefault(p => p.Primary).Number.Contains(searchText));

            if (!string.IsNullOrWhiteSpace(orderBy))
                baseQuery = baseQuery.OrderBySelector(new SortSelector(orderBy));

            IEntityQuery<ResourceListItem> query =
                baseQuery.Select(resource => new ResourceListItem
                                                 {
                                                     Id = resource.Id,
                                                     FirstName = resource.FirstName,
                                                     MiddleName = resource.MiddleName,
                                                     LastName = resource.LastName,
                                                     Address1 = resource.Addresses.FirstOrDefault(a => a.Primary).Address1,
                                                     Address2 = resource.Addresses.FirstOrDefault(a => a.Primary).Address2,
                                                     City = resource.Addresses.FirstOrDefault(a => a.Primary).City,
                                                     State = resource.Addresses.FirstOrDefault(a => a.Primary).State.ShortName,
                                                     Zipcode = resource.Addresses.FirstOrDefault(a => a.Primary).Zipcode,
                                                     AreaCode = resource.PhoneNumbers.FirstOrDefault(p => p.Primary).AreaCode,
                                                     Number = resource.PhoneNumbers.FirstOrDefault(p => p.Primary).Number
                                                 });

            return ExecuteQuery(query, onSuccess, onFail);
        }

        public OperationResult DeleteResourceAsync(Guid resourceId, Action onSuccess = null,
                                                    Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => DeleteResourceCore(resourceId), op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult();
        }

        public OperationResult GetStatesAsync(Action<IEnumerable<State>> onSuccess = null, Action<Exception> onFail = null)
        {
            IEntityQuery<State> query = Manager.States.OrderBy(s => s.Name).With(BaseDataQueryStrategy);
            return ExecuteQuery(query, onSuccess, onFail);
        }

        #endregion

        private IEnumerable<INotifyCompleted> DeleteResourceCore(Guid resourceId)
        {
            Resource resource = null;
            yield return GetResourceAsync(resourceId, result => resource = result);

            EntityAspect.Wrap(resource).Delete();
            yield return Manager.SaveChangesAsync();
        }

        private IEnumerable<INotifyCompleted> CreateResourceCore(string firstName, string middleName, string lastName)
        {
            Resource resource = Resource.Create();
            resource.FirstName = firstName;
            resource.MiddleName = middleName;
            resource.LastName = lastName;
            Manager.AddEntity(resource);

            AddressType addressType = null;
            yield return Manager.AddressTypes.Where(t => t.Default).With(BaseDataQueryStrategy)
                .ExecuteAsync(op => op.OnComplete(result => addressType = result.First(), null));
            resource.AddAddress(addressType);
            resource.PrimaryAddress = resource.Addresses.First();

            PhoneNumberType phoneType = null;
            yield return Manager.PhoneNumberTypes.Where(t => t.Default).With(BaseDataQueryStrategy)
                .ExecuteAsync(op => op.OnComplete(result => phoneType = result.First(), null));
            resource.AddPhoneNumber(phoneType);
            resource.PrimaryPhoneNumber = resource.PhoneNumbers.First();

            yield return Coroutine.Return(resource);
        }
    }
}