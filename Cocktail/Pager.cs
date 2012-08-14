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
        ///   Gets the zero-based index of the current page.
        /// </summary>
        public int CurrentPageIndex
        {
            get { return _entityQueryPager.PageIndex; }
        }

        /// <summary>
        ///   Returns the number of records requested for each page.
        /// </summary>
        public int PageSize
        {
            get { return _entityQueryPager.PageSize; }
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

                var fraction = (double)TotalItemCount/PageSize;
                return (int) Math.Ceiling(fraction);
            }
        }

        /// <summary>
        ///   Moves to the first page.
        /// </summary>
        /// <param name="onSuccess"> An optional callback to be called when the page was successfully retrieved. </param>
        /// <param name="onFail"> An optional callback to be called when the page retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        public PageOperationResult<T> FirstPageAsync(Action<Page<T>> onSuccess = null, Action<Exception> onFail = null)
        {
            return _entityQueryPager.MoveToFirstPageAsync()
                .OnComplete(onSuccess, onFail)
                .AsOperationResult();
        }

        /// <summary>
        ///   Moves to the last page.
        /// </summary>
        /// <param name="onSuccess"> An optional callback to be called when the page was successfully retrieved. </param>
        /// <param name="onFail"> An optional callback to be called when the page retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        public PageOperationResult<T> LastPageAsync(Action<Page<T>> onSuccess = null, Action<Exception> onFail = null)
        {
            return _entityQueryPager.MoveToLastPageAsync()
                .OnComplete(onSuccess, onFail)
                .AsOperationResult();
        }

        /// <summary>
        ///   Moves to the page after the current page.
        /// </summary>
        /// <param name="onSuccess"> An optional callback to be called when the page was successfully retrieved. </param>
        /// <param name="onFail"> An optional callback to be called when the page retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        public PageOperationResult<T> NextPageAsync(Action<Page<T>> onSuccess = null, Action<Exception> onFail = null)
        {
            return _entityQueryPager.MoveToNextPageAsync()
                .OnComplete(onSuccess, onFail)
                .AsOperationResult();
        }

        /// <summary>
        ///   Moves to the page before the current page.
        /// </summary>
        /// <param name="onSuccess"> An optional callback to be called when the page was successfully retrieved. </param>
        /// <param name="onFail"> An optional callback to be called when the page retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        public PageOperationResult<T> PreviousPageAsync(Action<Page<T>> onSuccess = null,
                                                        Action<Exception> onFail = null)
        {
            return _entityQueryPager.MoveToPreviousPageAsync()
                .OnComplete(onSuccess, onFail)
                .AsOperationResult();
        }

        /// <summary>
        ///   Moves to the specified page.
        /// </summary>
        /// <param name="pageIndex"> The zero-based index of the requested page. </param>
        /// <param name="onSuccess"> An optional callback to be called when the page was successfully retrieved. </param>
        /// <param name="onFail"> An optional callback to be called when the page retrieval failed. </param>
        /// <returns> Asynchronous operation result. </returns>
        public PageOperationResult<T> GoToPageAsync(int pageIndex, Action<Page<T>> onSuccess = null,
                                                    Action<Exception> onFail = null)
        {
            return _entityQueryPager.MoveToPageAsync(pageIndex)
                .OnComplete(onSuccess, onFail)
                .AsOperationResult();
        }

        #endregion
    }
}