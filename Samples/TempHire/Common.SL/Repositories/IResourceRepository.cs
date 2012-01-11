using System;
using System.Collections.Generic;
using DomainModel;
using DomainModel.Projections;
using IdeaBlade.EntityModel;

namespace Common.Repositories
{
    public interface IResourceRepository : IRepository
    {
        INotifyCompleted CreateResourceAsync(string firstName, string middleName, string lastName,
                                             Action<Resource> onSuccess = null, Action<Exception> onFail = null);

        INotifyCompleted GetAllResourcesAsync(Action<IEnumerable<Resource>> onSuccess = null,
                                              Action<Exception> onFail = null);

        INotifyCompleted GetResourceAsync(Guid resourceId, Action<Resource> onSuccess = null,
                                          Action<Exception> onFail = null);

        INotifyCompleted GetAddressTypesAsync(Action<IEnumerable<AddressType>> onSuccess = null,
                                              Action<Exception> onFail = null);

        INotifyCompleted GetPhoneTypesAsync(Action<IEnumerable<PhoneNumberType>> onSuccess = null,
                                            Action<Exception> onFail = null);

        INotifyCompleted GetRateTypesAsync(Action<IEnumerable<RateType>> onSuccess = null,
                                           Action<Exception> onFail = null);

        INotifyCompleted GetStatesAsync(Action<IEnumerable<State>> onSuccess = null, Action<Exception> onFail = null);

        INotifyCompleted FindResourcesAsync(string searchText, string orderBy,
                                            Action<IEnumerable<ResourceListItem>> onSuccess = null,
                                            Action<Exception> onFail = null);

        INotifyCompleted DeleteResourceAsync(Guid resourceId, Action onSuccess = null, Action<Exception> onFail = null);
    }
}