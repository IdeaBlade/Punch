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
using IdeaBlade.Application.Framework.Core.Composition;

namespace IdeaBlade.Application.Framework.Core.Persistence
{
    /// <summary>The RepositoryManager allows for the creation and sharing of multiple repository instances at runtime.</summary>
    /// <typeparam name="TKey">The type of the dictionary key used to retrieve a shared repository.</typeparam>
    /// <typeparam name="TRepository">The type of the repository.</typeparam>
    /// <example>
    /// When developing sandbox editing scenarios, you may find yourself having to create multiple instances of a repository as well as sharing an
    /// instance among multiple composed view models. The RepositoryManager pattern addresses this dilemma by managing instances of repositories with an associated key.
    /// <code title="OrderEditor" description="In this example we show a sandbox order editor that uses the OrderID as the key to find the associated repository. If the order editor were to be composed of multiple view models, as long as each view model works on the same order and uses the same OrderID to retrieve the repository, they can share the repository without needing to know about each other. The common connection between them is simply the order that is being editied." lang="CS">
    /// [Export(typeof(IOrderManagementRepository))]
    /// public class OrderManagementRepository : IOrderManagementRepository
    /// {
    ///     private readonly IEntityManagerProvider&lt;NorthwindIBEntities&gt; _entityManagerProvider;
    ///  
    ///     // Ensure that each repository instance receives a new instance of the EntityManagerProvider.
    ///     // This is the basic step to create a sandbox environment for each repository.
    ///     [ImportingConstructor]
    ///     public OrderManagementRepository(
    ///         [Import(RequiredCreationPolicy = CreationPolicy.NonShared)] IEntityManagerProvider&lt;NorthwindIBEntities&gt;
    ///             entityManagerProvider)
    ///     {
    ///         _entityManagerProvider = entityManagerProvider;
    ///     }
    ///  
    ///     private NorthwindIBEntities Manager
    ///     {
    ///         get { return _entityManagerProvider.Manager; }
    ///     }
    ///  
    ///     public Order ImportOrder(Order order)
    ///     {
    ///         Manager.ImportEntities(new[] { order }, MergeStrategy.OverwriteChanges);
    ///         return (Order)Manager.FindEntity(order.EntityAspect.EntityKey);
    ///     }
    ///  
    ///     // Add methods to perform operations against the persistence layer
    /// }
    ///  
    /// public interface IOrderManagmentRepositoryManager
    /// {
    ///     IOrderManagementRepository GetRepository(int key);
    /// }
    ///  
    /// // CreationPolicy of Shared ensures that the repository manager will be created as a singleton
    /// [Export(typeof(IOrderManagmentRepositoryManager)), PartCreationPolicy(CreationPolicy.Shared)]
    /// public class OrderManagementRepositoryManager : RepositoryManager&lt;int, IOrderManagementRepository&gt;,
    ///                                                 IOrderManagmentRepositoryManager
    /// {
    /// }
    ///  
    /// // Export this part non-shared. We want to edit in a sandbox
    /// [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    /// public class PerformOrderSave
    /// {
    ///     private readonly IOrderManagmentRepositoryManager _repositoryManager;
    ///  
    ///     // Inject repository manager in order to create new and share repositories
    ///     [ImportingConstructor]
    ///     public PerformOrderSave(IOrderManagmentRepositoryManager repositoryManager)
    ///     {
    ///         _repositoryManager = repositoryManager;
    ///     }
    ///  
    ///     private IOrderManagementRepository _repository;
    ///  
    ///     private IOrderManagementRepository GetRepository(Order order)
    ///     {
    ///         // Return the sandbox repository associated with the given order
    ///         return _repository ?? (_repository = _repositoryManager.GetRepository(order.OrderID));
    ///     }
    ///  
    ///     private Order _currentOrder;
    ///     public Order CurrentOrder
    ///     {
    ///         get { return _currentOrder; }
    ///         set
    ///         {
    ///             _repository = null; // Let's make sure we are not hanging on to an old repository
    ///             // Import the current order entity into the sandbox, so we can edit and save it.
    ///             _currentOrder = GetRepository(value).ImportOrder(value);
    ///         }
    ///     }
    ///  
    ///     // Add logic and methods to edit the current order
    /// }</code></example>
    [Obsolete("Use ObjectManager class instead")]
    public abstract class RepositoryManager<TKey, TRepository> : ObjectManager<TKey, TRepository>
    {
        /// <summary>Retrieves a  repository by key. If the key hasn't been encountered before, a new repository will be created.</summary>
        /// <param name="key">The key used to look up the repository. If the key is encountered for the first time, a new repository will be created.</param>
        /// <returns>An existing or new repository matching the key.</returns>
        public TRepository GetRepository(TKey key)
        {
            return GetObject(key);
        }
    }
}
