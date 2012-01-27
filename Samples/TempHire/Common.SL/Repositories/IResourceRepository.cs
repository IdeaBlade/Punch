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