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
        /// <param name="onSuccess"> Callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        OperationResult<T> WithIdAsync(object keyValue, Action<T> onSuccess = null, Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValue"> The single primary key value. </param>
        /// <param name="onSuccess"> Callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        OperationResult<T> WithIdFromDataSourceAsync(object keyValue, Action<T> onSuccess = null,
                                                     Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves the entity matching the provided key with the repository's default query strategy.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <param name="onSuccess"> Callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        OperationResult<T> WithIdAsync(object[] keyValues, Action<T> onSuccess = null, Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the back-end data source.
        /// </summary>
        /// <param name="keyValues"> The composite primary key values. </param>
        /// <param name="onSuccess"> Callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        /// <exception cref="EntityNotFoundException">A single entity matching the provided key was not found.</exception>
        OperationResult<T> WithIdFromDataSourceAsync(object[] keyValues, Action<T> onSuccess = null,
                                                     Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves the entity matching the provided key from the entity cache.
        /// </summary>
        /// <param name="keyValues"> The primary key values. </param>
        /// <returns> The retrieved entity. </returns>
        T WithIdFromCache(params object[] keyValues);

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription with the repository's default query strategy.
        /// </summary>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of entities </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable<T>> FindAsync(IPredicateDescription predicateDescription = null,
                                                  ISortSelector sortSelector = null,
                                                  string includeProperties = null,
                                                  Action<IEnumerable<T>> onSuccess = null,
                                                  Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription with the repository's default query strategy and projects the results into a different shape using the projectionSelector parameter.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable> FindAsync(IProjectionSelector projectionSelector,
                                               IPredicateDescription predicateDescription = null,
                                               ISortSelector sortSelector = null,
                                               Action<IEnumerable> onSuccess = null,
                                               Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the back-end data source.
        /// </summary>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of entities </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable<T>> FindInDataSourceAsync(IPredicateDescription predicateDescription = null,
                                                              ISortSelector sortSelector = null,
                                                              string includeProperties = null,
                                                              Action<IEnumerable<T>> onSuccess = null,
                                                              Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the back-end data source and projects the results into a different shape using the projectionSelector parameter.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable> FindInDataSourceAsync(IProjectionSelector projectionSelector,
                                               IPredicateDescription predicateDescription = null,
                                               ISortSelector sortSelector = null,
                                               Action<IEnumerable> onSuccess = null,
                                               Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the cache.
        /// </summary>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of entities </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of entities. </param>
        /// <returns> The list of retrieved entities. </returns>
        IEnumerable<T> FindInCache(IPredicateDescription predicateDescription = null,
                                   ISortSelector sortSelector = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided predicateDescription from the cache and projects the results into a different shape using the projectionSelector parameter.
        /// </summary>
        /// <param name="projectionSelector"> The selector used to shape the result.</param>
        /// <param name="predicateDescription"> Optional predicate description to filter the returned list of objects. </param>
        /// <param name="sortSelector"> Optional sort descriptor to sort the returned list of objects.. </param>
        /// <returns> The list of retrieved objects. </returns>
        IEnumerable FindInCache(IProjectionSelector projectionSelector,
                                IPredicateDescription predicateDescription = null,
                                ISortSelector sortSelector = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate = null,
                                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                  string includeProperties = null,
                                                  Action<IEnumerable<T>> onSuccess = null,
                                                  Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression with the repository's default query strategy and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result.</param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable<TResult>> FindAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IEnumerable<TResult>> onSuccess = null, Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable<T>> FindInDataSourceAsync(Expression<Func<T, bool>> predicate = null,
                                                              Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                              string includeProperties = null,
                                                              Action<IEnumerable<T>> onSuccess = null,
                                                              Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the back-end data source and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result.</param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <param name="onSuccess"> Optional callback to be called when the entity retrieval was successful. </param>
        /// <param name="onFail"> Optional callback to be called when the entity retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        OperationResult<IEnumerable<TResult>> FindInDataSourceAsync<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null,
            Action<IEnumerable<TResult>> onSuccess = null, Action<Exception> onFail = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache.
        /// </summary>
        /// <param name="predicate"> Optional predicate to filter the returned list of entities </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of entities. </param>
        /// <returns> The list of retrieved entities. </returns>
        IEnumerable<T> FindInCache(Expression<Func<T, bool>> predicate = null,
                                   Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        /// <summary>
        ///   Retrieves one or more entities matching the provided expression from the cache and projects the results into a different shape using the selector parameter.
        /// </summary>
        /// <param name="selector"> The selector used to shape the result.</param>
        /// <param name="predicate"> Optional predicate to filter the returned list of objects. </param>
        /// <param name="orderBy"> Optional sorting function to sort the returned list of objects. </param>
        /// <returns> The list of retrieved objects. </returns>
        IEnumerable<TResult> FindInCache<TResult>(
            Func<IQueryable<T>, IQueryable<TResult>> selector, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy = null);

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
        /// Returns true if the entity matching the provided key is found in the cache.
        /// </summary>
        /// <param name="keyValues">The primary key values</param>
        bool ExistsInCache(params object[] keyValues);
    }
}