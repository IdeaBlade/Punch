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

namespace Common.Repositories
{
    public interface IStaffingResourceRepository : IRepository
    {
        OperationResult<StaffingResource> CreateStaffingResourceAsync(
            string firstName, string middleName, string lastName, Action<StaffingResource> onSuccess = null,
            Action<Exception> onFail = null);

        OperationResult<IEnumerable<StaffingResource>> GetAllStaffingResourcesAsync(
            Action<IEnumerable<StaffingResource>> onSuccess = null, Action<Exception> onFail = null);

        OperationResult<IEnumerable<StaffingResource>> GetStaffingResourceAsync(
            Guid staffingResourceId, Action<StaffingResource> onSuccess = null, Action<Exception> onFail = null);

        OperationResult<IEnumerable<AddressType>> GetAddressTypesAsync(
            Action<IEnumerable<AddressType>> onSuccess = null, Action<Exception> onFail = null);

        OperationResult<IEnumerable<PhoneNumberType>> GetPhoneTypesAsync(
            Action<IEnumerable<PhoneNumberType>> onSuccess = null, Action<Exception> onFail = null);

        OperationResult<IEnumerable<RateType>> GetRateTypesAsync(
            Action<IEnumerable<RateType>> onSuccess = null, Action<Exception> onFail = null);

        OperationResult<IEnumerable<State>> GetStatesAsync(Action<IEnumerable<State>> onSuccess = null,
                                                           Action<Exception> onFail = null);

        OperationResult<IEnumerable<StaffingResourceListItem>> FindStaffingResourcesAsync(
            string searchText, string orderBy, Action<IEnumerable<StaffingResourceListItem>> onSuccess = null,
            Action<Exception> onFail = null);

        OperationResult DeleteStaffingResourceAsync(Guid staffingResourceId, Action onSuccess = null,
                                                    Action<Exception> onFail = null);
    }
}