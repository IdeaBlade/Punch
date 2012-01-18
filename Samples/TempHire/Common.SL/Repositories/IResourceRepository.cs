using System;
using System.Collections.Generic;
using Cocktail;
using DomainModel;
using DomainModel.Projections;
using Action = System.Action;

namespace Common.Repositories
{
    public interface IResourceRepository : IRepository
    {
        OperationResult CreateResourceAsync(string firstName, string middleName, string lastName,
                                             Action<Resource> onSuccess = null, Action<Exception> onFail = null);

        OperationResult GetAllResourcesAsync(Action<IEnumerable<Resource>> onSuccess = null,
                                              Action<Exception> onFail = null);

        OperationResult GetResourceAsync(Guid resourceId, Action<Resource> onSuccess = null,
                                          Action<Exception> onFail = null);

        OperationResult GetAddressTypesAsync(Action<IEnumerable<AddressType>> onSuccess = null,
                                              Action<Exception> onFail = null);

        OperationResult GetPhoneTypesAsync(Action<IEnumerable<PhoneNumberType>> onSuccess = null,
                                            Action<Exception> onFail = null);

        OperationResult GetRateTypesAsync(Action<IEnumerable<RateType>> onSuccess = null,
                                           Action<Exception> onFail = null);

        OperationResult GetStatesAsync(Action<IEnumerable<State>> onSuccess = null, Action<Exception> onFail = null);

        OperationResult FindResourcesAsync(string searchText, string orderBy,
                                            Action<IEnumerable<ResourceListItem>> onSuccess = null,
                                            Action<Exception> onFail = null);

        OperationResult DeleteResourceAsync(Guid resourceId, Action onSuccess = null, Action<Exception> onFail = null);
    }
}