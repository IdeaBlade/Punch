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

using Cocktail;
using Cocktail.Contrib.UnitOfWork;
using DomainModel;
using DomainServices.Repositories;

namespace DomainServices
{
    public abstract class UnitOfWorkCore : UnitOfWork, IUnitOfWorkCore
    {
        protected UnitOfWorkCore(IEntityManagerProvider entityManagerProvider, IPreLoader preLoader)
            : base(entityManagerProvider)
        {
            AddressTypes = new PreLoadRepository<AddressType>(entityManagerProvider, preLoader);
            PhoneNumberTypes = new PreLoadRepository<PhoneNumberType>(entityManagerProvider, preLoader);
            RateTypes = new PreLoadRepository<RateType>(entityManagerProvider, preLoader);
            States = new PreLoadRepository<State>(entityManagerProvider, preLoader);
        }

        #region IUnitOfWorkCore Members

        public IRepository<AddressType> AddressTypes { get; private set; }
        public IRepository<PhoneNumberType> PhoneNumberTypes { get; private set; }
        public IRepository<RateType> RateTypes { get; private set; }
        public IRepository<State> States { get; private set; }

        #endregion
    }
}