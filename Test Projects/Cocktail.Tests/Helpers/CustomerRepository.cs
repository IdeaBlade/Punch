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
using System.Threading.Tasks;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using Test.Model;

#if !NETFX_CORE
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif


namespace Cocktail.Tests.Helpers
{
    /// <summary>
    /// An example of a Repository.
    /// 
    /// IMPORTANT: The Repository must be exported via its type or an implemented interface.
    /// This example exports the Repository via ICustomerRepository. Exporting the Repository
    /// allows the CompositionHost to automatically plug it into any ViewModel that needs
    /// to use the Repository
    /// 
    /// <seealso cref="ICustomerRepository"/>
    /// </summary>
    [Export(typeof(ICustomerRepository))]
    public class CustomerRepository : ICustomerRepository
    {
        [ImportingConstructor]
        public CustomerRepository(IEntityManagerProvider<NorthwindIBEntities> entityManagerProvider)
        {
            EntityManagerProvider = entityManagerProvider;
            AddEventHandler();
        }

        protected internal IEntityManagerProvider<NorthwindIBEntities> EntityManagerProvider { get; set; }

        private NorthwindIBEntities Manager { get { return EntityManagerProvider.Manager; } }

        #region ICustomerRepository Members

        public event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// An example of a method to retrieve a collection of entities.
        /// </summary>
        /// <param name="orderByPropertyName"></param>
        /// <returns>List of customers</returns>
        public Task<IEnumerable<Customer>> GetCustomersAsync(string orderByPropertyName)
        {
            IEntityQuery<Customer> query = Manager.Customers;
            if (orderByPropertyName != null)
            {
                var selector = new SortSelector(typeof(Customer), orderByPropertyName);
                query = query.OrderBySelector(selector);
            }

            return query.ExecuteAsync();
        }

        public void AddCustomer(Customer customer)
        {
            Manager.AddEntity(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            customer.EntityAspect.Delete();
        }

        /// <summary>
        /// An example of a method to save pending changes.
        /// </summary>
        /// <returns>SaveResult</returns>
        public Task SaveAsync()
        {
            return Manager.SaveChangesAsync();
        }

        #endregion

        private void AddEventHandler()
        {
            EntityManagerProvider.DataChanged += (s, args) => { if (DataChanged != null) DataChanged(this, args); };
        }
    }
}