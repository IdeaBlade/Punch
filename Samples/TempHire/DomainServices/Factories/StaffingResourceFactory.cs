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
using System.Linq;
using Cocktail;
using Cocktail.Contrib.UnitOfWork;
using DomainModel;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;

namespace DomainServices.Factories
{
    public class StaffingResourceFactory : IStaffingResourceFactory
    {
        private readonly IRepository<AddressType> _addressTypes;
        private readonly IEntityManagerProvider<TempHireEntities> _entityManagerProvider;
        private readonly IRepository<PhoneNumberType> _phoneNumberTypes;

        public StaffingResourceFactory(IEntityManagerProvider<TempHireEntities> entityManagerProvider,
                                       IRepository<AddressType> addressTypes,
                                       IRepository<PhoneNumberType> phoneNumberTypes)
        {
            _entityManagerProvider = entityManagerProvider;
            _addressTypes = addressTypes;
            _phoneNumberTypes = phoneNumberTypes;
        }

        private TempHireEntities EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IStaffingResourceFactory Members

        public OperationResult<StaffingResource> CreateAsync(string firstName, string middleName, string lastName,
                                                             Action<StaffingResource> onSuccess = null,
                                                             Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => CreateAsyncCore(firstName, middleName, lastName),
                                   op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult<StaffingResource>();
        }

        #endregion

        private IEnumerable<INotifyCompleted> CreateAsyncCore(string firstName, string middleName, string lastName)
        {
            var staffingResource = StaffingResource.Create();
            staffingResource.FirstName = firstName;
            staffingResource.MiddleName = middleName;
            staffingResource.LastName = lastName;
            EntityManager.AddEntity(staffingResource);

            OperationResult<IEnumerable<AddressType>> op1;
            yield return
                op1 = _addressTypes.FindAsync(PredicateBuilder.Make("Default", FilterOperator.IsEqualTo, true));
            var addressType = op1.Result.First();
            staffingResource.AddAddress(addressType);
            staffingResource.PrimaryAddress = staffingResource.Addresses.First();

            OperationResult<IEnumerable<PhoneNumberType>> op2;
            yield return
                op2 = _phoneNumberTypes.FindAsync(PredicateBuilder.Make("Default", FilterOperator.IsEqualTo, true));
            var phoneType = op2.Result.First();
            staffingResource.AddPhoneNumber(phoneType);
            staffingResource.PrimaryPhoneNumber = staffingResource.PhoneNumbers.First();

            yield return Coroutine.Return(staffingResource);
        }
    }
}