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
        AsyncOperation CreateResourceAsync(string firstName, string middleName, string lastName,
                                             Action<Resource> onSuccess = null, Action<Exception> onFail = null);

        AsyncOperation GetAllResourcesAsync(Action<IEnumerable<Resource>> onSuccess = null,
                                              Action<Exception> onFail = null);

        AsyncOperation GetResourceAsync(Guid resourceId, Action<Resource> onSuccess = null,
                                          Action<Exception> onFail = null);

        AsyncOperation GetAddressTypesAsync(Action<IEnumerable<AddressType>> onSuccess = null,
                                              Action<Exception> onFail = null);

        AsyncOperation GetPhoneTypesAsync(Action<IEnumerable<PhoneNumberType>> onSuccess = null,
                                            Action<Exception> onFail = null);

        AsyncOperation GetRateTypesAsync(Action<IEnumerable<RateType>> onSuccess = null,
                                           Action<Exception> onFail = null);

        AsyncOperation GetStatesAsync(Action<IEnumerable<State>> onSuccess = null, Action<Exception> onFail = null);

        AsyncOperation FindResourcesAsync(string searchText, string orderBy,
                                            Action<IEnumerable<ResourceListItem>> onSuccess = null,
                                            Action<Exception> onFail = null);

        AsyncOperation DeleteResourceAsync(Guid resourceId, Action onSuccess = null, Action<Exception> onFail = null);
    }
}