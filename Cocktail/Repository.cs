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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///     A generic implementation of a repository.
    /// </summary>
    /// <typeparam name="T"> The type of entity this repository retrieves. </typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IEntityManagerProvider _entityManagerProvider;
        private QueryStrategy _defaultQueryStrategy;

        /// <summary>
        ///     Creates a new repository.
        /// </summary>
        /// <param name="entityManagerProvider"> The EntityMangerProvider to be used to obtain an EntityManager. </param>
        /// <param name="defaultQueryStrategy"> The optional default query strategy. </param>
        public Repository(IEntityManagerProvider entityManagerProvider, QueryStrategy defaultQueryStrategy = null)
        {
            _entityManagerProvider = entityManagerProvider;
            _defaultQueryStrategy = defaultQueryStrategy;
        }

        /// <summary>
        ///     Gets or sets the repository's default query strategy.
        /// </summary>
        public QueryStrategy DefaultQueryStrategy
        {
            get { return _defaultQueryStrategy ?? EntityManager.DefaultQueryStrategy; }
            set { _defaultQueryStrategy = value; }
        }

        /// <summary>
        ///     Returns the EntityManager used by this repository.
        /// </summary>
        protected EntityManager EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IRepository<T> Members

        /// <summary>
        ///     Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdAsync(object keyValue)
        {
            return WithIdAsync(new[] {keyValue});
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdAsync(object keyValue, CancellationToken cancellationToken)
        {
            return WithIdAsync(new[] {keyValue}, cancellationToken);
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdFromDataSourceAsync(object keyValue)
        {
            return WithIdFromDataSourceAsync(new[] {keyValue});
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdFromDataSourceAsync(object keyValue, CancellationToken cancellationToken)
        {
            return WithIdFromDataSourceAsync(new[] {keyValue}, cancellationToken);
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdAsync(object[] keyValues)
        {
            return WithIdAsync(keyValues, CancellationToken.None);
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            var query = GetKeyQuery(keyValues);
            return WithIdAsyncCore(query, cancellationToken);
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdFromDataSourceAsync(object[] keyValues)
        {
            return WithIdFromDataSourceAsync(keyValues, CancellationToken.None);
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdFromDataSourceAsync(object[] keyValues, CancellationToken cancellationToken)
        {
            var query = GetKeyQuery(keyValues).With(QueryStrategy.DataSourceOnly);
            return WithIdAsyncCore(query, cancellationToken);
        }

        /// <summary>
        ///     Retrieves the entity matching the provided key from the entity cache.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public T WithIdFromCache(params object[] keyValues)
        {
            var key = new EntityKey(typeof(T), keyValues);
            var entity = (T) EntityManager.FindEntity(key);
            if (entity == null)
                throw new EntityNotFoundException(ShouldHaveExactlyOneEntityErrorMessage(key.ToQuery(), 0));
            return entity;
        }

        /// <summary>
        ///     Retrieves all entities with the repository's default query strategy.
        /// </summary>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> AllAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                             Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindAsync(x => true, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves all entities with the repository's default query strategy.
        /// </summary>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> AllAsync(CancellationToken cancellationToken,
                                             Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                             Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindAsync(x => true, cancellationToken, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves all entities from the back-end data source.
        /// </summary>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> AllInDataSourceAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                         Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindInDataSourceAsync(x => true, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves all entities from the back-end data source.
        /// </summary>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> AllInDataSourceAsync(CancellationToken cancellationToken,
                                                         Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                         Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindInDataSourceAsync(x => true, cancellationToken, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves all entities from the cache.
        /// </summary>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public IEnumerable<T> AllInCache(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                         Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindInCache(x => true, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Returns the number of entities.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the entities </param>
        /// <returns> The number of entities matching the optional expression. </returns>
        public Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return GetFindQuery(predicate, null, null)
                .AsScalarAsync()
                .Count();
        }

        /// <summary>
        ///     Returns the number of entities in the cache.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the entities </param>
        /// <returns> The number of entities matching the optional expression. </returns>
        public int CountInCache(Expression<Func<T, bool>> predicate = null)
        {
            return GetFindQuery(predicate, null, null)
                .With(QueryStrategy.CacheOnly)
                .Count();
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression with the repository's default query strategy.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                              Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindAsync(predicate, CancellationToken.None, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression with the repository's default query strategy.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken,
                                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                              Action<IFetchOptions<T>> fetchOptions = null)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var query = GetFindQuery(predicate, orderBy, fetchOptions);
            return query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable<TResult>> FindAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null)
        {
            return FindAsync(selector, CancellationToken.None, predicate, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable<TResult>> FindAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var query = GetFindQuery(selector, predicate, orderBy, fetchOptions);
            return query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable> FindAsync(Func<IQueryable<T>, IQueryable> selector,
                                           Expression<Func<T, bool>> predicate = null,
                                           Func<IQueryable, IOrderedQueryable> orderBy = null,
                                           Action<IFetchOptions> fetchOptions = null)
        {
            return FindAsync(selector, CancellationToken.None, predicate, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable> FindAsync(Func<IQueryable<T>, IQueryable> selector, CancellationToken cancellationToken,
                                           Expression<Func<T, bool>> predicate = null,
                                           Func<IQueryable, IOrderedQueryable> orderBy = null,
                                           Action<IFetchOptions> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var query = GetFindQuery(selector, predicate, orderBy, fetchOptions);
            return query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindInDataSourceAsync(Expression<Func<T, bool>> predicate,
                                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                          Action<IFetchOptions<T>> fetchOptions = null)
        {
            return FindInDataSourceAsync(predicate, CancellationToken.None, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindInDataSourceAsync(Expression<Func<T, bool>> predicate,
                                                          CancellationToken cancellationToken,
                                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                          Action<IFetchOptions<T>> fetchOptions = null)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            var query = GetFindQuery(predicate, orderBy, fetchOptions).With(QueryStrategy.DataSourceOnly);
            return query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable<TResult>> FindInDataSourceAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null)
        {
            return FindInDataSourceAsync(selector, CancellationToken.None, predicate, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable<TResult>> FindInDataSourceAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var query = GetFindQuery(selector, predicate, orderBy, fetchOptions).With(QueryStrategy.DataSourceOnly);
            return query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable> FindInDataSourceAsync(Func<IQueryable<T>, IQueryable> selector,
                                                       Expression<Func<T, bool>> predicate = null,
                                                       Func<IQueryable, IOrderedQueryable> orderBy = null,
                                                       Action<IFetchOptions> fetchOptions = null)
        {
            return FindInDataSourceAsync(selector, CancellationToken.None, predicate, orderBy, fetchOptions);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable> FindInDataSourceAsync(Func<IQueryable<T>, IQueryable> selector,
                                                       CancellationToken cancellationToken,
                                                       Expression<Func<T, bool>> predicate = null,
                                                       Func<IQueryable, IOrderedQueryable> orderBy = null,
                                                       Action<IFetchOptions> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var query = GetFindQuery(selector, predicate, orderBy, fetchOptions).With(QueryStrategy.DataSourceOnly);
            return query.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the cache.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        public IEnumerable<T> FindInCache(Expression<Func<T, bool>> predicate,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                          Action<IFetchOptions<T>> fetchOptions = null)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return GetFindQuery(predicate, orderBy, fetchOptions).With(QueryStrategy.CacheOnly).Execute();
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the cache and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public IEnumerable<TResult> FindInCache<TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector,
                                                         Expression<Func<T, bool>> predicate = null,
                                                         Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy =
                                                             null, Action<IFetchOptions<TResult>> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            return GetFindQuery(selector, predicate, orderBy, fetchOptions).With(QueryStrategy.CacheOnly).Execute();
        }

        /// <summary>
        ///     Retrieves one or more entities matching the provided expression from the cache and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        public IEnumerable FindInCache(Func<IQueryable<T>, IQueryable> selector,
                                       Expression<Func<T, bool>> predicate = null,
                                       Func<IQueryable, IOrderedQueryable> orderBy = null,
                                       Action<IFetchOptions> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            return GetFindQuery(selector, predicate, orderBy, fetchOptions).With(QueryStrategy.CacheOnly).Execute();
        }

        /// <summary>
        ///     Marks the specified entity as to be deleted.
        /// </summary>
        /// <param name="entity"> Entity to be deleted. </param>
        public void Delete(T entity)
        {
            EntityAspect.Wrap(entity).Delete();
        }

        /// <summary>
        ///     Marks the specified entities as to be deleted.
        /// </summary>
        /// <param name="entities"> Entities to be deleted. </param>
        public void Delete(IEnumerable<T> entities)
        {
            entities.ForEach(Delete);
        }

        /// <summary>
        ///     Returns true if the entity matching the provided key is found in the cache.
        /// </summary>
        /// <param name="keyValues"> The primary key values </param>
        public bool ExistsInCache(params object[] keyValues)
        {
            var key = new EntityKey(typeof(T), keyValues);
            return EntityManager.FindEntity(key) != null;
        }

        #endregion

        /// <summary>
        ///     Returns the query to retrieve a single entity,
        /// </summary>
        /// <param name="keyValues"> One ore more primary key values. </param>
        /// <remarks>
        ///     Override to modify the query used to retrieve a single entity
        /// </remarks>
        protected virtual IEntityQuery GetKeyQuery(params object[] keyValues)
        {
            return new EntityKey(typeof(T), keyValues).ToQuery().With(EntityManager, DefaultQueryStrategy);
        }

        /// <summary>
        ///     Returns the query to retrieve a list of entities.
        /// </summary>
        /// <param name="predicate"> The predicate expression used to qualify the list of entities. </param>
        /// <param name="orderBy"> Sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Delegate to specify additional fetching options.</param>
        /// <remarks>
        ///     Override to modify the query used to retrieve a list of entities
        /// </remarks>
        protected virtual IEntityQuery<T> GetFindQuery(Expression<Func<T, bool>> predicate,
                                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                                                       Action<IFetchOptions<T>> fetchOptions)
        {
            IEntityQuery<T> query = EntityManager.GetQuery<T>();
            if (predicate != null)
                query = query.Where(predicate);
            if (orderBy != null)
                query = (IEntityQuery<T>) orderBy(query);

            if (fetchOptions != null)
            {
                var options = new FetchOptions<T>(query);
                fetchOptions(options);
                query = options.Query;
            }

            return query.With(DefaultQueryStrategy);
        }

        /// <summary>
        ///     Returns the strongly typed query to retrieve a list of projected entities.
        /// </summary>
        /// <param name="selector"> The selector used to project the entities. </param>
        /// <param name="predicate"> The predicate expression used to qualify the list of objects. </param>
        /// <param name="orderBy"> Sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Delegate to specify additional fetching options.</param>
        /// <remarks>
        ///     Override to modify the query used to retrieve a list of objects.
        /// </remarks>
        protected virtual IEntityQuery<TResult> GetFindQuery<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy,
            Action<IFetchOptions<TResult>> fetchOptions)
        {
            IEntityQuery<T> baseQuery = EntityManager.GetQuery<T>();
            if (predicate != null)
                baseQuery = baseQuery.Where(predicate);

            var query = (IEntityQuery<TResult>) selector(baseQuery);
            if (orderBy != null)
                query = (IEntityQuery<TResult>) orderBy(query);

            if (fetchOptions != null)
            {
                var options = new FetchOptions<TResult>(query);
                fetchOptions(options);
                query = options.Query;
            }

            return query.With(DefaultQueryStrategy);
        }

        /// <summary>
        ///     Returns the query to retrieve a list of projected entities.
        /// </summary>
        /// <param name="selector"> The selector used to project the entities. </param>
        /// <param name="predicate"> The predicate expression used to qualify the list of objects. </param>
        /// <param name="orderBy"> Sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Delegate to specify additional fetching options.</param>
        /// <remarks>
        ///     Override to modify the query used to retrieve a list of objects.
        /// </remarks>
        protected virtual IEntityQuery GetFindQuery(
            Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate,
            Func<IQueryable, IOrderedQueryable> orderBy, Action<IFetchOptions> fetchOptions)
        {
            IEntityQuery<T> baseQuery = EntityManager.GetQuery<T>();
            if (predicate != null)
                baseQuery = baseQuery.Where(predicate);

            var query = selector(baseQuery);
            if (orderBy != null)
                query = orderBy(query);

            if (fetchOptions != null)
            {
                var options = new FetchOptions((ITypedEntityQuery) query);
                fetchOptions(options);
                query = (IQueryable) options.Query;
            }

            return ((IEntityQuery) query).With(DefaultQueryStrategy);
        }

        private async Task<T> WithIdAsyncCore(IEntityQuery entityQuery, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entities =
                (await EntityManager.ExecuteQueryAsync(entityQuery, cancellationToken)).Cast<object>().ToList();

            if (entities.Count != 1)
                throw new EntityNotFoundException(ShouldHaveExactlyOneEntityErrorMessage(entityQuery, entities.Count));

            return (T) entities.First();
        }

        private string ShouldHaveExactlyOneEntityErrorMessage(IEntityQuery entityQuery, int count)
        {
            return string.Format(StringResources.ShouldHaveExactlyOneEntityErrorMessage, typeof(T), entityQuery, count);
        }
    }

    internal class FetchOptions : IFetchOptions
    {
        public FetchOptions(ITypedEntityQuery query)
        {
            Query = query;
        }

        public ITypedEntityQuery Query { get; private set; }

        public IFetchOptions Skip(int count)
        {
            Query = Query.Skip(count);
            return this;
        }

        public IFetchOptions Take(int count)
        {
            Query = Query.Take(count);
            return this;
        }

        public IFetchOptions Distinct()
        {
            Query = Query.Distinct();
            return this;
        }

        public IFetchOptions Include(string propertyPath)
        {
            Query = Query.Include(propertyPath);
            return this;
        }
    }

    internal class FetchOptions<T> : IFetchOptions<T>
    {
        public FetchOptions(IEntityQuery<T> query)
        {
            if (query == null) throw new ArgumentNullException("query");
            Query = query;
        }

        public IEntityQuery<T> Query { get; private set; }

        public IFetchOptions<T> Take(int count)
        {
            Query = Query.Take(count);
            return this;
        }

        public IFetchOptions<T> Distinct()
        {
            Query = Query.Distinct();
            return this;
        }

        public IFetchOptions<T> Include(string propertyPath)
        {
            Query = Query.Include(propertyPath);
            return this;
        }

        public IFetchOptions<T> Include(Expression<Func<T, object>> expr)
        {
            Query = Query.Include(expr);
            return this;
        }

        public IFetchOptions<T> Skip(int count)
        {
            Query = Query.Skip(count);
            return this;
        }
    }
}