//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using Test.Model;

namespace Cocktail.Tests.Helpers
{
    /// <summary>
    /// An example of a Repository.
    /// 
    /// IMPORTANT: The Repository must be exported via its type or an implemented interface.
    /// This example exports the Respository via ICustomerRepository. Exporting the Repository
    /// allows the CompositionHost to automatically plug it into any ViewModel that needs
    /// to use the Repository
    /// 
    /// <seealso cref="ICustomerRepository"/>
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        [ImportingConstructor]
        public CustomerRepository(IEntityManagerProvider<NorthwindIBEntities> entityManagerProvider)
        {
            EntityManagerProvider = entityManagerProvider;
            AddEventHandler();
        }

        protected internal IEntityManagerProvider<NorthwindIBEntities> EntityManagerProvider { get; set; }

        protected internal CompositionContext Context { get { return EntityManagerProvider.Context; } }

        private NorthwindIBEntities Manager { get { return EntityManagerProvider.Manager; } }

        #region ICustomerRepository Members

        public event EventHandler<DataChangedEventArgs> DataChanged;

        /// <summary>
        /// An example of a method to retrieve a collection of entities.
        /// </summary>
        /// <param name="orderByPropertyName"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns>An object to notify completion. Facilitates the use of this operation in a Coroutine.</returns>
        public INotifyCompleted GetCustomers(string orderByPropertyName, Action<IEnumerable<Customer>> onSuccess = null, Action<Exception> onFail = null)
        {
            IEntityQuery<Customer> query = Manager.Customers;
            if (orderByPropertyName != null)
            {
                var selector = new SortSelector(typeof(Customer), orderByPropertyName);
                query = query.OrderBySelector(selector);
            }

            EntityQueryOperation<Customer> op = query.ExecuteAsync();
            return op.OnComplete(onSuccess, onFail);
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
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns>An object to notify completion. Facilitates the use of this operation in a Coroutine.</returns>
        public INotifyCompleted Save(Action onSuccess = null, Action<Exception> onFail = null)
        {
            EntitySaveOperation op = Manager.SaveChangesAsync();
            return op.OnComplete(onSuccess, onFail);
        }

        #endregion

        private void AddEventHandler()
        {
            EntityManagerProvider.DataChanged += (s, args) => { if (DataChanged != null) DataChanged(this, args); };
        }
    }
}