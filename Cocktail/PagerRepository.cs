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
using System.Linq;
using System.Linq.Expressions;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///     A generic implementation of a repository which allows paging of entity and object results.
    /// </summary>
    /// <typeparam name="T"> The type of entity this repository retrieves. </typeparam>
    public class PagerRepository<T> : Repository<T>, IPagerRepository<T> where T : class
    {
        /// <summary>
        ///     Creates a new repository.
        /// </summary>
        /// <param name="entityManagerProvider"> The EntityMangerProvider to be used to obtain an EntityManager. </param>
        /// <param name="defaultQueryStrategy"> The optional default query strategy. </param>
        public PagerRepository(IEntityManagerProvider entityManagerProvider, QueryStrategy defaultQueryStrategy = null)
            : base(entityManagerProvider, defaultQueryStrategy)
        {
        }

        #region IPagerRepository<T> Members

        /// <summary>
        ///     Returns a pager which allows entities to be paged.
        /// </summary>
        /// <param name="sortSelector"> Required sorting criteria. </param>
        /// <param name="pageSize"> The desired page size. </param>
        /// <param name="predicate"> Optional predicate to filter the paged entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <returns>
        ///     <see cref="IPager{T}" /> which allows the entities to be paged.
        /// </returns>
        public IPager<T> Pager(ISortSelector sortSelector, int pageSize, Expression<Func<T, bool>> predicate = null,
                               Action<IFetchOptions<T>> fetchOptions = null)
        {
            if (sortSelector == null)
                throw new ArgumentNullException("sortSelector");

            var query = GetFindQuery(predicate, null, fetchOptions);
            return new Pager<T>(query, sortSelector, pageSize);
        }

        /// <summary>
        ///     Returns a pager which allows shaped entities to be paged.
        /// </summary>
        /// <param name="selector"> The selector used to shape the entities. </param>
        /// <param name="pageSize"> The desired page size. </param>
        /// <param name="sortSelector"> Required sorting criteria. </param>
        /// <param name="predicate"> Optional predicate to filter the paged entities. </param>
        /// <param name="fetchOptions">Optional delegate to specify additional fetching options.</param>
        /// <typeparam name="TResult"> The shape of the result. </typeparam>
        /// <returns>
        ///     <see cref="IPager{T}" /> which allows the shaped entities to be paged.
        /// </returns>
        public IPager<TResult> Pager<TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, int pageSize,
                                              ISortSelector sortSelector, Expression<Func<T, bool>> predicate = null,
                                              Action<IFetchOptions<TResult>> fetchOptions = null)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (sortSelector == null)
                throw new ArgumentNullException("sortSelector");

            var query = GetFindQuery(selector, predicate, null, fetchOptions);

            // If TResult is not an entity type, then the query strategy must be set to DataSourceOnly
            // otherwise the EntityQueryPager doesn't return any data. This appears to be a bug in DevForce.
            if (!EntityManager.IsEntityType(typeof(TResult)))
                query = query.With(QueryStrategy.DataSourceOnly);

            return new Pager<TResult>(query, sortSelector, pageSize);
        }

        #endregion
    }
}