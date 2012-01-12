using System;
using System.Collections.Generic;
using Caliburn.Micro;
using DomainModel;
using DomainModel.Projections;
using Action = System.Action;

namespace Common.Repositories
{
    public interface IResourceRepository : IRepository
    {
        IResult CreateResourceAsync(string firstName, string middleName, string lastName,
                                             Action<Resource> onSuccess = null, Action<Exception> onFail = null);

        IResult GetAllResourcesAsync(Action<IEnumerable<Resource>> onSuccess = null,
                                              Action<Exception> onFail = null);

        IResult GetResourceAsync(Guid resourceId, Action<Resource> onSuccess = null,
                                          Action<Exception> onFail = null);

        IResult GetAddressTypesAsync(Action<IEnumerable<AddressType>> onSuccess = null,
                                              Action<Exception> onFail = null);

        IResult GetPhoneTypesAsync(Action<IEnumerable<PhoneNumberType>> onSuccess = null,
                                            Action<Exception> onFail = null);

        IResult GetRateTypesAsync(Action<IEnumerable<RateType>> onSuccess = null,
                                           Action<Exception> onFail = null);

        IResult GetStatesAsync(Action<IEnumerable<State>> onSuccess = null, Action<Exception> onFail = null);

        IResult FindResourcesAsync(string searchText, string orderBy,
                                            Action<IEnumerable<ResourceListItem>> onSuccess = null,
                                            Action<Exception> onFail = null);

        IResult DeleteResourceAsync(Guid resourceId, Action onSuccess = null, Action<Exception> onFail = null);
    }
}