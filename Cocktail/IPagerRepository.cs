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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>
    ///   Represents a single page returned by <see cref="IPager{T}" />
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    public class Page<T>
    {
        /// <summary>
        ///   Creates and initializes a new Page.
        /// </summary>
        /// <param name="pageIndex"> The index of the page moved to. </param>
        /// <param name="pageWasFound"> Indicates whether the page was found. </param>
        /// <param name="results"> The data associated with the current page. </param>
        public Page(int pageIndex, bool pageWasFound, IEnumerable<T> results)
        {
            PageIndex = pageIndex;
            PageWasFound = pageWasFound;
            Results = results;
        }

        /// <summary>
        ///   The index of the page moved to.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        ///   Returns false when the requested page was not found.
        /// </summary>
        public bool PageWasFound { get; private set; }

        /// <summary>
        ///   The data associated with the current page.
        /// </summary>
        public IEnumerable<T> Results { get; private set; }
    }

    /// <summary>
    ///   A service which allows paging of entity and object results.
    /// </summary>
    public interface IPager<T>
    {
        /// <summary>
        ///   Returns the number of records requested for each page.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Returns true if a page change is in progress.
        /// </summary>
        bool IsPageChanging { get; }

        /// <summary>
        ///   Returns the number of records available to be returned from the back-end data source.
        /// </summary>
        /// <remarks>
        ///   This property will return -1 until the last page is fetched via a call to LastPageAsync() /> .
        /// </remarks>
        int TotalDataSourceItemCount { get; }

        /// <summary>
        ///   Returns the total number of items returned.
        /// </summary>
        /// <remarks>
        ///   This property will return -1 until all pages are fetched or the <see cref="TotalDataSourceItemCount" /> is known. The number will be an approximation when a large number of inserts and deletes have occurred.
        /// </remarks>
        int TotalItemCount { get; }

        /// <summary>
        /// Returns the total number of pages.
        /// </summary>
        /// <remarks>
        ///   This property will return -1 until all pages are fetched or the <see cref="TotalDataSourceItemCount" /> is known. The number will be an approximation when a large number of inserts and deletes have occurred.
        /// </remarks>
        int TotalNumberOfPages { get; }

        /// <summary>
        /// Returns the current page.
        /// </summary>
        Page<T> CurrentPage { get; }

        /// <summary>
        ///   Moves to the first page.
        /// </summary>
        /// <returns> The first page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> FirstPageAsync();

        /// <summary>
        ///   Moves to the first page.
        /// </summary>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns> The first page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> FirstPageAsync(CancellationToken cancellationToken);

        /// <summary>
        ///   Moves to the last page.
        /// </summary>
        /// <returns> The last page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> LastPageAsync();

        /// <summary>
        ///   Moves to the last page.
        /// </summary>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns> The last page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> LastPageAsync(CancellationToken cancellationToken);

        /// <summary>
        ///   Moves to the page after the current page.
        /// </summary>
        /// <returns> The next page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> NextPageAsync();

        /// <summary>
        ///   Moves to the page after the current page.
        /// </summary>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns> The next page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> NextPageAsync(CancellationToken cancellationToken);

        /// <summary>
        ///   Moves to the page before the current page.
        /// </summary>
        /// <returns> The previous page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> PreviousPageAsync();

        /// <summary>
        ///   Moves to the page before the current page.
        /// </summary>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns> The previous page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> PreviousPageAsync(CancellationToken cancellationToken);

        /// <summary>
        ///   Moves to the specified page.
        /// </summary>
        /// <param name="pageIndex"> The zero-based index of the requested page. </param>
        /// <returns> The requested page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> GoToPageAsync(int pageIndex);

        /// <summary>
        ///   Moves to the specified page.
        /// </summary>
        /// <param name="pageIndex"> The zero-based index of the requested page. </param>
        /// <param name="cancellationToken">A token that allows for the operation to be cancelled.</param>
        /// <returns> The requested page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        Task<Page<T>> GoToPageAsync(int pageIndex, CancellationToken cancellationToken);
    }

    /// <summary>
    ///   Interface to be implemented by a repository supporting paging.
    /// </summary>
    /// <typeparam name="T"> The type of entity this repository retrieves. </typeparam>
    public interface IPagerRepository<T> : IRepository<T> where T : class
    {
        /// <summary>
        ///   Returns a pager which allows entities to be paged.
        /// </summary>
        /// <param name="sortSelector"> Required sorting criteria. </param>
        /// <param name="pageSize"> The desired page size. </param>
        /// <param name="predicate"> Optional predicate to filter the paged entities. </param>
        /// <param name="includeProperties"> Optional related entities to eager fetch together with the returned list of entities. Use comma to separate multiple properties. </param>
        /// <returns> <see cref="IPager{T}" /> which allows the entities to be paged. </returns>
        IPager<T> Pager(ISortSelector sortSelector, int pageSize, Expression<Func<T, bool>> predicate = null,
                        string includeProperties = null);

        /// <summary>
        ///   Returns a pager which allows shaped entities to be paged.
        /// </summary>
        /// <param name="selector"> The selector used to shape the entities. </param>
        /// <param name="pageSize"> The desired page size. </param>
        /// <param name="sortSelector"> Required sorting criteria. </param>
        /// <param name="predicate"> Optional predicate to filter the paged entities. </param>
        /// <typeparam name="TResult"> The shape of the result. </typeparam>
        /// <returns> <see cref="IPager{T}" /> which allows the shaped entities to be paged. </returns>
        IPager<TResult> Pager<TResult>(Func<IQueryable<T>, IQueryable<TResult>> selector, int pageSize,
                                       ISortSelector sortSelector, Expression<Func<T, bool>> predicate = null);
    }
}