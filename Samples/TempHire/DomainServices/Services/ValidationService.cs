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
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IRepository<StaffingResource> _staffingResources;

        public ValidationService(IRepository<StaffingResource> staffingResources)
        {
            _staffingResources = staffingResources;
        }

        #region IValidationService Members

        public OperationResult<bool> CheckIfDuplicateAsync(StaffingResource staffingResource,
                                                           Action<bool> onSuccess = null,
                                                           Action<Exception> onFail = null)
        {
            return Coroutine.Start(() => CheckIfDuplicateCore(staffingResource), op => op.OnComplete(onSuccess, onFail))
                .AsOperationResult<bool>();
        }

        #endregion

        private IEnumerable<INotifyCompleted> CheckIfDuplicateCore(StaffingResource staffingResource)
        {
            OperationResult<IEnumerable<Guid>> operation;
            yield return operation = _staffingResources.FindAsync(q => q.Select(x => x.Id),
                                                                  x =>
                                                                  x.Id != staffingResource.Id &&
                                                                  x.FirstName == staffingResource.FirstName &&
                                                                  x.MiddleName == staffingResource.MiddleName &&
                                                                  x.LastName == staffingResource.LastName);
            yield return Coroutine.Return(operation.Result.Any());
        }
    }
}