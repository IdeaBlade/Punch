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
using Cocktail;
using DomainModel;
using IdeaBlade.EntityModel;

namespace DomainServices.Repositories
{
    public class StaffingResourceRepository : Repository<StaffingResource>
    {
        public StaffingResourceRepository(IEntityManagerProvider<TempHireEntities> entityManagerProvider)
            : base(entityManagerProvider)
        {
        }

        public new TempHireEntities EntityManager
        {
            get { return (TempHireEntities) base.EntityManager; }
        }

        protected override IEntityQuery GetKeyQuery(params object[] keyValues)
        {
            return EntityManager.StaffingResources
                .Where(r => r.Id == (Guid) keyValues[0])
                .Include(r => r.Addresses)
                .Include(r => r.PhoneNumbers);
        }
    }
}