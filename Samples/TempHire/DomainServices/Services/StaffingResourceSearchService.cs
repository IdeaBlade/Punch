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
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cocktail;
using DomainModel;
using DomainModel.Projections;

namespace DomainServices.Services
{
    public class StaffingResourceSearchService : IStaffingResourceSearchService
    {
        private readonly IRepository<StaffingResource> _repository;

        public StaffingResourceSearchService(IRepository<StaffingResource> repository)
        {
            _repository = repository;
        }

        #region IStaffingResourceSearchService Members

        public Task<IEnumerable<StaffingResourceListItem>> Simple(string text)
        {
            return Simple(text, CancellationToken.None);
        }

        public Task<IEnumerable<StaffingResourceListItem>> Simple(string text, CancellationToken cancellationToken)
        {
            Expression<Func<StaffingResource, bool>> filter = null;
            if (!string.IsNullOrWhiteSpace(text))
                filter = x => x.FirstName.Contains(text) ||
                              x.MiddleName.Contains(text) ||
                              x.LastName.Contains(text) ||
                              x.Addresses.FirstOrDefault(a => a.Primary).Address1.Contains(text) ||
                              x.Addresses.FirstOrDefault(a => a.Primary).Address2.Contains(text) ||
                              x.Addresses.FirstOrDefault(a => a.Primary).City.Contains(text) ||
                              x.Addresses.FirstOrDefault(a => a.Primary).Zipcode.Contains(text) ||
                              x.Addresses.FirstOrDefault(a => a.Primary).State.Name.Contains(text) ||
                              x.Addresses.FirstOrDefault(a => a.Primary).State.ShortName.Contains(text) ||
                              x.PhoneNumbers.FirstOrDefault(p => p.Primary).AreaCode.Contains(text) ||
                              x.PhoneNumbers.FirstOrDefault(p => p.Primary).Number.Contains(text);

            return _repository.FindAsync(
                q => q.Select(x => new StaffingResourceListItem
                                       {
                                           Id = x.Id,
                                           FirstName = x.FirstName,
                                           MiddleName = x.MiddleName,
                                           LastName = x.LastName,
                                           Address1 = x.Addresses.FirstOrDefault(a => a.Primary).Address1,
                                           Address2 = x.Addresses.FirstOrDefault(a => a.Primary).Address2,
                                           City = x.Addresses.FirstOrDefault(a => a.Primary).City,
                                           State = x.Addresses.FirstOrDefault(a => a.Primary).State.ShortName,
                                           Zipcode = x.Addresses.FirstOrDefault(a => a.Primary).Zipcode,
                                           AreaCode = x.PhoneNumbers.FirstOrDefault(p => p.Primary).AreaCode,
                                           Number = x.PhoneNumbers.FirstOrDefault(p => p.Primary).Number
                                       }),
                cancellationToken, filter, q => q.OrderBy(i => i.LastName));
        }

        #endregion
    }
}