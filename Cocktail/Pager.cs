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
using System.Threading.Tasks;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    internal class Pager<T> : IPager<T>
    {
        private readonly EntityQueryPager<T> _entityQueryPager;

        public Pager(IEntityQuery<T> baseQuery, ISortSelector sortSelector, int pageSize)
        {
            _entityQueryPager = EntityQueryPager.Create(baseQuery, sortSelector, pageSize);
        }

        #region IPager<T> Members

        /// <summary>
        ///   Returns the number of records requested for each page.
        /// </summary>
        public int PageSize
        {
            get { return _entityQueryPager.PageSize; }
        }

        /// <summary>
        /// Returns true if a page change is in progress.
        /// </summary>
        public bool IsPageChanging
        {
            get { return _entityQueryPager.IsPageChanging; }
        }

        /// <summary>
        ///   Returns the number of records available to be returned from the back-end data source.
        /// </summary>
        /// <remarks>
        ///   This property will return -1 until the last page is fetched via a call to <see cref="IPager{T}.LastPageAsync" /> .
        /// </remarks>
        public int TotalDataSourceItemCount
        {
            get { return _entityQueryPager.TotalDataSourceItemCount; }
        }

        /// <summary>
        ///   Returns the total number of items returned.
        /// </summary>
        /// <remarks>
        ///   This property will return -1 until all pages are fetched or the <see cref="IPager{T}.TotalDataSourceItemCount" /> is known. The number will be an approximation when a large number of inserts and deletes have occurred.
        /// </remarks>
        public int TotalItemCount
        {
            get { return _entityQueryPager.TotalItemCount; }
        }

        /// <summary>
        /// Returns the total number of pages.
        /// </summary>
        /// <remarks>
        ///   This property will return -1 until all pages are fetched or the <see cref="IPager{T}.TotalDataSourceItemCount" /> is known. The number will be an approximation when a large number of inserts and deletes have occurred.
        /// </remarks>
        public int TotalNumberOfPages
        {
            get
            {
                if (TotalItemCount == -1)
                    return -1;

                var fraction = (double)TotalItemCount / PageSize;
                return (int)Math.Ceiling(fraction);
            }
        }

        /// <summary>
        /// Returns the current page.
        /// </summary>
        public Page<T> CurrentPage
        {
            get { return new Page<T>(_entityQueryPager.PageIndex, true, _entityQueryPager.CurrentPageResults); }
        }

        /// <summary>
        ///   Moves to the first page.
        /// </summary>
        /// <returns> The first page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        public async Task<Page<T>> FirstPageAsync()
        {
            ThrowIfPageChanging();

            var pageFound = await _entityQueryPager.MoveToFirstPageAsync();
            if (pageFound)
                return CurrentPage;

            return new Page<T>(0, false, new List<T>());
        }

        /// <summary>
        ///   Moves to the last page.
        /// </summary>
        /// <returns> The last page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        public async Task<Page<T>> LastPageAsync()
        {
            ThrowIfPageChanging();

            var pageFound = await _entityQueryPager.MoveToLastPageAsync();
            if (pageFound)
                return CurrentPage;

            return new Page<T>(TotalNumberOfPages - 1, false, new List<T>());
        }

        /// <summary>
        ///   Moves to the page after the current page.
        /// </summary>
        /// <returns> The next page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        public async Task<Page<T>> NextPageAsync()
        {
            ThrowIfPageChanging();

            var pageFound = await _entityQueryPager.MoveToNextPageAsync();
            if (pageFound)
                return CurrentPage;

            return new Page<T>(_entityQueryPager.PageIndex + 1, false, new List<T>());
        }

        /// <summary>
        ///   Moves to the page before the current page.
        /// </summary>
        /// <returns> The previous page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        public async Task<Page<T>> PreviousPageAsync()
        {
            ThrowIfPageChanging();

            var pageFound = await _entityQueryPager.MoveToPreviousPageAsync();
            if (pageFound)
                return CurrentPage;

            return new Page<T>(_entityQueryPager.PageIndex - 1, false, new List<T>());
        }

        /// <summary>
        ///   Moves to the specified page.
        /// </summary>
        /// <param name="pageIndex"> The zero-based index of the requested page. </param>
        /// <returns> The requested page. </returns>
        /// <exception cref="InvalidOperationException">A page change is in progress.</exception>
        public async Task<Page<T>> GoToPageAsync(int pageIndex)
        {
            ThrowIfPageChanging();

            var pageFound = await _entityQueryPager.MoveToPageAsync(pageIndex);
            if (pageFound)
                return CurrentPage;

            return new Page<T>(pageIndex, false, new List<T>());
        }

        #endregion

        private void ThrowIfPageChanging()
        {
            if (IsPageChanging)
                throw new InvalidOperationException(StringResources.PageChangeInProgress);
        }
    }
}