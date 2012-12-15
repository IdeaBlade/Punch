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

namespace Cocktail
{
    /// <summary>
    ///   Interface to be implemented by a repository.
    /// </summary>
    /// <typeparam name="T"> The type of entity this repository retrieves. </typeparam>
    public interface IRepository<T> : IHideObjectMembers where T : class
    {
        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdAsync(object keyValue);

        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdAsync(object keyValue, CancellationToken cancellationToken);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdFromDataSourceAsync(object keyValue);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdFromDataSourceAsync(object keyValue, CancellationToken cancellationToken);

        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdAsync(object[] keyValues);

        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdAsync(object[] keyValues, CancellationToken cancellationToken);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdFromDataSourceAsync(object[] keyValues);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        Task<T> WithIdFromDataSourceAsync(object[] keyValues, CancellationToken cancellationToken);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the entity cache.
        /// </summary>
        /// <param name="keyValues"> The primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        T WithIdFromCache(params object[] keyValues);

        /// <summary>
        ///   Retrieves all entities with the repository's default query strategy.
        /// </summary>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> AllAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                      Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves all entities with the repository's default query strategy.
        /// </summary>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> AllAsync(CancellationToken cancellationToken,
                                      Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                      Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves all entities from the back-end data source.
        /// </summary>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> AllInDataSourceAsync(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                  Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves all entities from the back-end data source.
        /// </summary>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> AllInDataSourceAsync(CancellationToken cancellationToken,
                                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                  Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves all entities from the cache.
        /// </summary>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        IEnumerable<T> AllInCache(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                  Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Returns the number of entities.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the entities </param>
        /// <returns> The number of entities matching the optional expression. </returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        ///   Returns the number of entities in the cache.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the entities </param>
        /// <returns> The number of entities matching the optional expression. </returns>
        int CountInCache(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                       Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
                                       CancellationToken cancellationToken,
                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                       Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable<TResult>> FindAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable<TResult>> FindAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable> FindAsync(
            Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable, IOrderedQueryable> orderBy = null, Action<IFetchOptions> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable> FindAsync(
            Func<IQueryable<T>, IQueryable> selector, CancellationToken cancellationToken,
            Expression<Func<T, bool>> predicate = null, Func<IQueryable, IOrderedQueryable> orderBy = null,
            Action<IFetchOptions> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> FindInDataSourceAsync(Expression<Func<T, bool>> predicate,
                                                   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                   Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        Task<IEnumerable<T>> FindInDataSourceAsync(Expression<Func<T, bool>> predicate,
                                                   CancellationToken cancellationToken,
                                                   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                   Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable<TResult>> FindInDataSourceAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable<TResult>> FindInDataSourceAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, CancellationToken cancellationToken,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable> FindInDataSourceAsync(
            Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable, IOrderedQueryable> orderBy = null, Action<IFetchOptions> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="cancellationToken"> A token that allows for the operation to be cancelled. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        Task<IEnumerable> FindInDataSourceAsync(
            Func<IQueryable<T>, IQueryable> selector, CancellationToken cancellationToken,
            Expression<Func<T, bool>> predicate = null, Func<IQueryable, IOrderedQueryable> orderBy = null,
            Action<IFetchOptions> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved entities. </returns>
        IEnumerable<T> FindInCache(Expression<Func<T, bool>> predicate,
                                   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                   Action<IFetchOptions<T>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        IEnumerable<TResult> FindInCache<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IFetchOptions<TResult>> fetchOptions = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result. </param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns> The list of retrieved objects. </returns>
        IEnumerable FindInCache(
            Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable, IOrderedQueryable> orderBy = null, Action<IFetchOptions> fetchOptions = null);

        /// <summary>
        ///   Marks the specified entity as to be deleted.
        /// </summary>
        /// <param name="entity"> Entity to be deleted. </param>
        void Delete(T entity);

        /// <summary>
        ///   Marks the specified entities as to be deleted.
        /// </summary>
        /// <param name="entities"> Entities to be deleted. </param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        ///   Returns true if the entity matching the provided key is found in the cache.
        /// </summary>
        /// <param name="keyValues"> The primary key values </param>
        bool ExistsInCache(params object[] keyValues);
    }

    /// <summary>
    /// A fluent interface to specify additional data fetching options.
    /// </summary>
    public interface IFetchOptions : IHideObjectMembers
    {
        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        IFetchOptions Skip(int count);

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns></returns>
        IFetchOptions Take(int count);

        /// <summary>
        /// Returns distinct elements from a sequence.
        /// </summary>
        IFetchOptions Distinct();

        /// <summary>
        /// Configures eager fetching for related entities in the specified query path.
        /// </summary>
        /// <param name="propertyPath">Dot-separated list of navigation properties that describe the query path in the graph that should be eagerly fetched.</param>
        IFetchOptions Include(string propertyPath);
    }

    /// <summary>
    /// A fluent generic interface to specify additional data fetching options.
    /// </summary>
    public interface IFetchOptions<T>
    {
        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        IFetchOptions<T> Skip(int count);

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="count">The number of elements to return.</param>
        /// <returns></returns>
        IFetchOptions<T> Take(int count);

        /// <summary>
        /// Returns distinct elements from a sequence.
        /// </summary>
        IFetchOptions<T> Distinct();

        /// <summary>
        /// Configures eager fetching for related entities in the specified query path.
        /// </summary>
        /// <param name="propertyPath">Dot-separated list of navigation properties that describe the query path in the graph that should be eagerly fetched.</param>
        IFetchOptions<T> Include(string propertyPath);

        /// <summary>
        /// Configures eager fetching for related entities in the specified query path.
        /// </summary>
        /// <param name="expr">An expression returning the navigation property.</param>
        /// <returns></returns>
        IFetchOptions<T> Include(Expression<Func<T, object>> expr);
    }
}