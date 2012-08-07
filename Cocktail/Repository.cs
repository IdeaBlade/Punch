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
using System.Threading.Tasks;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using IdeaBlade.Linq;
using EntityKey = IdeaBlade.EntityModel.EntityKey;

namespace Cocktail
{
    /// <summary>
    ///   A generic implementation of a repository.
    /// </summary>
    /// <typeparam name="T"> The type of entity this repository retrieves. </typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IEntityManagerProvider _entityManagerProvider;
        private QueryStrategy _defaultQueryStrategy;

        /// <summary>
        ///   Creates a new repository.
        /// </summary>
        /// <param name="entityManagerProvider"> The EntityMangerProvider to be used to obtain an EntityManager. </param>
        public Repository(IEntityManagerProvider entityManagerProvider)
        {
            _entityManagerProvider = entityManagerProvider;
        }

        /// <summary>
        ///   Gets or sets the default QueryStrategy for all queries performed by the current repository.
        /// </summary>
        public QueryStrategy DefaultQueryStrategy
        {
            get { return _defaultQueryStrategy ?? EntityManager.DefaultQueryStrategy; }
            set { _defaultQueryStrategy = value; }
        }

        /// <summary>
        ///   Returns the EntityManager used by this repository.
        /// </summary>
        protected EntityManager EntityManager
        {
            get { return _entityManagerProvider.Manager; }
        }

        #region IRepository<T> Members

        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdAsync(object keyValue)
        {
            return WithIdAsync(new[] { keyValue });
        }

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdFromDataSourceAsync(object keyValue)
        {
            return WithIdFromDataSourceAsync(new[] { keyValue });
        }

        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdAsync(object[] keyValues)
        {
            var query = GetKeyQuery(keyValues);
            return WithIdAsyncCore(query);
        }

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public Task<T> WithIdFromDataSourceAsync(object[] keyValues)
        {
            var query = GetKeyQuery(keyValues).With(QueryStrategy.DataSourceOnly);
            return WithIdAsyncCore(query);
        }

        /// <summary>
        ///   Retrieves the entity matching the provided key from the entity cache.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        public T WithIdFromCache(params object[] keyValues)
        {
            var key = new EntityKey(typeof(T), keyValues);
            var entity = (T)EntityManager.FindEntity(key);
            if (entity == null)
                throw new EntityNotFoundException(ShouldHaveExactlyOneEntityErrorMessage(key.ToQuery(), 0));
            return entity;
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription with the repository's default query strategy.
        /// </summary>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of entities </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindAsync(IPredicateDescription predicateDescription = null,
                                              ISortSelector sortSelector = null,
                                              string includeProperties = null)
        {
            Expression<Func<T, bool>> predicate = null;
            if (predicateDescription != null)
                predicate = predicateDescription.ToPredicate<T>();
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null;
            if (sortSelector != null)
                orderBy = q => q.OrderBySelector(sortSelector);

            return FindAsync(predicate, orderBy, includeProperties);
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription with the repository's default query strategy and projects the results into a different shape using the projectionSelector parameter.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable> FindAsync(IProjectionSelector projectionSelector,
                                           IPredicateDescription predicateDescription = null,
                                           ISortSelector sortSelector = null)
        {
            if (projectionSelector == null)
                throw new ArgumentNullException("projectionSelector");

            var query = GetFindQuery(projectionSelector, predicateDescription, sortSelector);
            return query.ExecuteAsync();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the back-end data source.
        /// </summary>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of entities </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindInDataSourceAsync(IPredicateDescription predicateDescription = null,
                                                          ISortSelector sortSelector = null,
                                                          string includeProperties = null)
        {
            Expression<Func<T, bool>> predicate = null;
            if (predicateDescription != null)
                predicate = predicateDescription.ToPredicate<T>();
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null;
            if (sortSelector != null)
                orderBy = q => q.OrderBySelector(sortSelector);

            return FindInDataSourceAsync(predicate, orderBy, includeProperties);
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the back-end data source and projects the results into a different shape using the projectionSelector parameter.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable> FindInDataSourceAsync(IProjectionSelector projectionSelector,
                                                       IPredicateDescription predicateDescription = null,
                                                       ISortSelector sortSelector = null)
        {
            if (projectionSelector == null)
                throw new ArgumentNullException("projectionSelector");

            var query =
                GetFindQuery(projectionSelector, predicateDescription, sortSelector).With(QueryStrategy.DataSourceOnly);
            return query.ExecuteAsync();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the cache.
        /// </summary>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of entities </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of entities. </param>
        /// <returns> The list of retrieved entities. </returns>
        public IEnumerable<T> FindInCache(IPredicateDescription predicateDescription = null,
                                          ISortSelector sortSelector = null)
        {
            Expression<Func<T, bool>> predicate = null;
            if (predicateDescription != null)
                predicate = predicateDescription.ToPredicate<T>();
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null;
            if (sortSelector != null)
                orderBy = q => q.OrderBySelector(sortSelector);

            return FindInCache(predicate, orderBy);
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the cache and projects the results into a different shape using the projectionSelector parameter.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        public IEnumerable FindInCache(IProjectionSelector projectionSelector,
                                       IPredicateDescription predicateDescription = null,
                                       ISortSelector sortSelector = null)
        {
            if (projectionSelector == null)
                throw new ArgumentNullException("projectionSelector");

            var query =
                GetFindQuery(projectionSelector, predicateDescription, sortSelector).With(QueryStrategy.CacheOnly);
            return query.Execute();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate = null,
                                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                              string includeProperties = null)
        {
            var query = GetFindQuery(predicate, orderBy, includeProperties);
            return query.ExecuteAsync();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable<TResult>> FindAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var query = GetFindQuery(selector, predicate, orderBy);
            return query.ExecuteAsync();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <returns> The list of retrieved entities. </returns>
        public Task<IEnumerable<T>> FindInDataSourceAsync(
            Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null)
        {
            var query = GetFindQuery(predicate, orderBy, includeProperties).With(QueryStrategy.DataSourceOnly);
            return query.ExecuteAsync();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result.</param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        public Task<IEnumerable<TResult>> FindInDataSourceAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            var query = GetFindQuery(selector, predicate, orderBy).With(QueryStrategy.DataSourceOnly);
            return query.ExecuteAsync();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <returns> The list of retrieved entities. </returns>
        public IEnumerable<T> FindInCache(Expression<Func<T, bool>> predicate = null,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            return GetFindQuery(predicate, orderBy, null).With(QueryStrategy.CacheOnly).ToList();
        }

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result.</param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        public IEnumerable<TResult> FindInCache<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            return GetFindQuery(selector, predicate, orderBy).With(QueryStrategy.CacheOnly).ToList();
        }

        /// <summary>
        ///   Marks the specified entity as to be deleted.
        /// </summary>
        /// <param name="entity"> Entity to be deleted. </param>
        public void Delete(T entity)
        {
            EntityAspect.Wrap(entity).Delete();
        }

        /// <summary>
        ///   Marks the specified entities as to be deleted.
        /// </summary>
        /// <param name="entities"> Entities to be deleted. </param>
        public void Delete(IEnumerable<T> entities)
        {
            entities.ToList().ForEach(Delete);
        }

        /// <summary>
        /// Returns true if the entity matching the provided key is found in the cache.
        /// </summary>
        /// <param name="keyValues">The primary key values</param>
        public bool ExistsInCache(params object[] keyValues)
        {
            var key = new EntityKey(typeof(T), keyValues);
            return EntityManager.FindEntity(key) != null;
        }

        #endregion

        /// <summary>
        ///   Returns the query to retrieve a single entity,
        /// </summary>
        /// <param name="keyValues"> One ore more primary key values. </param>
        /// <remarks>
        ///   Override to modify the query used to retrieve a single entity
        /// </remarks>
        protected virtual IEntityQuery GetKeyQuery(params object[] keyValues)
        {
            return new EntityKey(typeof(T), keyValues).ToQuery().With(EntityManager, DefaultQueryStrategy);
        }

        /// <summary>
        ///   Returns the query to retrieve a list of entities.
        /// </summary>
        /// <param name="predicate"> The predicate expression used to qualify the list of entities. </param>
        /// <param name="orderBy"> Sorting function to sort the returned list of entities. </param>
        /// <param name="includeProperties"> An optional list of included properties. </param>
        /// <remarks>
        ///   Override to modify the query used to retrieve a list of entities
        /// </remarks>
        protected virtual IEntityQuery<T> GetFindQuery(Expression<Func<T, bool>> predicate,
                                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                                                       string includeProperties)
        {
            IEntityQuery<T> query = EntityManager.GetQuery<T>();
            if (predicate != null)
                query = query.Where(predicate);
            if (orderBy != null)
                query = (IEntityQuery<T>)orderBy(query);
            if (!string.IsNullOrWhiteSpace(includeProperties))
                query = ParseIncludeProperties(includeProperties)
                    .Aggregate(query, (q, includeProperty) => q.Include(includeProperty));

            return query.With(DefaultQueryStrategy);
        }

        /// <summary>
        ///   Returns the strongly typed query to retrieve a list of projected entities.
        /// </summary>
        /// <param name="selector"> The selector used to project the entities. </param>
        /// <param name="predicate"> The predicate expression used to qualify the list of objects. </param>
        /// <param name="orderBy"> Sorting function to sort the returned list of objects. </param>
        /// <remarks>
        ///   Override to modify the query used to retrieve a list of objects.
        /// </remarks>
        protected virtual IEntityQuery<TResult> GetFindQuery<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy)
        {
            IEntityQuery<T> baseQuery = EntityManager.GetQuery<T>();
            if (predicate != null)
                baseQuery = baseQuery.Where(predicate);

            var query = (IEntityQuery<TResult>)selector(baseQuery);
            if (orderBy != null)
                query = (IEntityQuery<TResult>)orderBy(query);

            return query.With(DefaultQueryStrategy);
        }

        /// <summary>
        ///   Returns the dynamic query to retrieve a list of projected entities.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects. </param>
        /// <remarks>
        ///   Override to modify the query used to retrieve a list of objects.
        /// </remarks>
        protected virtual IEntityQuery GetFindQuery(IProjectionSelector projectionSelector,
                                                    IPredicateDescription predicateDescription,
                                                    ISortSelector sortSelector)
        {
            IEntityQuery<T> baseQuery = EntityManager.GetQuery<T>();
            if (predicateDescription != null)
                baseQuery = baseQuery.Where(predicateDescription);

            var query = baseQuery.Select(projectionSelector);
            if (sortSelector != null)
                query = query.OrderBySelector(sortSelector);

            return query.With(DefaultQueryStrategy);
        }

        private async Task<T> WithIdAsyncCore(IEntityQuery entityQuery)
        {
            var entities = (await EntityManager.ExecuteQueryAsync(entityQuery)).Cast<object>().ToList();

            if (entities.Count != 1)
                throw new EntityNotFoundException(ShouldHaveExactlyOneEntityErrorMessage(entityQuery, entities.Count));

            return (T)entities.First();
        }

        private IEnumerable<string> ParseIncludeProperties(string includeProperties)
        {
            return includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string ShouldHaveExactlyOneEntityErrorMessage(IEntityQuery entityQuery, int count)
        {
            return string.Format(StringResources.ShouldHaveExactlyOneEntityErrorMessage, typeof(T), entityQuery, count);
        }
    }
}