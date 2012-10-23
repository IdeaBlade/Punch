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
using Caliburn.Micro;
using IdeaBlade.EntityModel;
using Action = System.Action;

namespace Cocktail
{
    /// <summary>
    ///   Extension methods to provide support for the legacy asynchronous API.
    /// </summary>
    public static class Compatibility
    {
        // Legacy support for IAuthenticationService

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult LoginAsync(
            this IAuthenticationService source, ILoginCredential credential, Action onSuccess, Action<Exception> onFail)
        {
            return source.LoginAsync(credential).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult LogoutAsync(this IAuthenticationService source, Action callback)
        {
            return source.LogoutAsync().OnComplete(callback, null);
        }

        // Legacy support for IDialogManager

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> ShowDialogAsync<T>(
            this IDialogManager source, object content, IEnumerable<T> dialogButtons, string title)
        {
            return source.ShowDialogAsync(content, dialogButtons, title);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> ShowDialogAsync<T>(
            this IDialogManager source, object content, T defaultButton, T cancelButton, IEnumerable<T> dialogButtons,
            string title)
        {
            return source.ShowDialogAsync(content, defaultButton, cancelButton, dialogButtons, title);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<DialogResult> ShowDialogAsync(
            this IDialogManager source, object content, IEnumerable<DialogResult> dialogButtons, string title)
        {
            return source.ShowDialogAsync(content, dialogButtons, title);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> ShowMessageAsync<T>(
            this IDialogManager source, string message, IEnumerable<T> dialogButtons, string title)
        {
            return source.ShowMessageAsync(message, dialogButtons, title);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> ShowMessageAsync<T>(
            this IDialogManager source, string message, T defaultButton, T cancelButton, IEnumerable<T> dialogButtons,
            string title)
        {
            return source.ShowMessageAsync(message, defaultButton, cancelButton, dialogButtons, title);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<DialogResult> ShowMessageAsync(
            this DialogManager source, string message, IEnumerable<DialogResult> dialogButtons, string title)
        {
            return source.ShowMessageAsync(message, dialogButtons, title);
        }

        // Legacy support for IFactory<T>

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> CreateAsync<T>(
            this IFactory<T> source, Action<T> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.CreateAsync().OnComplete(onSuccess, onFail);
        }

        // Legacy support for Fake Backing Store

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult InitializeFakeBackingStoreAsync<T>(
            this IEntityManagerProvider<T> source, Action onSuccess, Action<Exception> onFail)
            where T : EntityManager
        {
            return source.InitializeFakeBackingStoreAsync().OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult ResetFakeBackingStoreAsync<T>(
            this IEntityManagerProvider<T> source, Action onSuccess, Action<Exception> onFail)
            where T : EntityManager
        {
            return source.ResetFakeBackingStoreAsync().OnComplete(onSuccess, onFail);
        }

        // Legacy support for IPager

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static PageOperationResult<T> FirstPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.FirstPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static PageOperationResult<T> LastPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.LastPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static PageOperationResult<T> NextPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.NextPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static PageOperationResult<T> PreviousPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.PreviousPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static PageOperationResult<T> GoToPageAsync<T>(
            this IPager<T> source, int pageIndex, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.GoToPageAsync(pageIndex, cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        // Legacy support for IRepository<T>

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> WithIdAsync<T>(
            this IRepository<T> source, object keyValue, Action<T> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.WithIdAsync(keyValue).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> WithIdFromDataSourceAsync<T>(
            this IRepository<T> source, object keyValue, Action<T> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.WithIdFromDataSourceAsync(keyValue).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> WithIdAsync<T>(
            this IRepository<T> source, object[] keyValues, Action<T> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.WithIdAsync(keyValues).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<T> WithIdFromDataSourceAsync<T>(
            this IRepository<T> source, object[] keyValues, Action<T> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.WithIdFromDataSourceAsync(keyValues).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable<T>> AllAsync<T>(
            this IRepository<T> source, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties,
            Action<IEnumerable<T>> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.AllAsync(orderBy, includeProperties).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable<T>> AllInDataSourceAsync<T>(
            this IRepository<T> source, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties,
            Action<IEnumerable<T>> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.AllInDataSourceAsync(orderBy, includeProperties).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable<T>> FindAsync<T>(
            this IRepository<T> source, Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties,
            Action<IEnumerable<T>> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.FindAsync(predicate, orderBy, includeProperties).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable<TResult>> FindAsync<T, TResult>(
            this IRepository<T> source, Func<IQueryable<T>, IQueryable<TResult>> selector,
            Expression<Func<T, bool>> predicate, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy,
            Action<IEnumerable<TResult>> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.FindAsync(selector, predicate, orderBy).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable> FindAsync<T>(
            this IRepository<T> source, Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate,
            Func<IQueryable, IOrderedQueryable> orderBy, Action<IEnumerable> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.FindAsync(selector, predicate, orderBy).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable<T>> FindInDataSourceAsync<T>(
            this IRepository<T> source, Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties,
            Action<IEnumerable<T>> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.FindInDataSourceAsync(predicate, orderBy, includeProperties).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable<TResult>> FindInDataSourceAsync<T, TResult>(
            this IRepository<T> source, Func<IQueryable<T>, IQueryable<TResult>> selector,
            Expression<Func<T, bool>> predicate, Func<IQueryable<TResult>, IOrderedQueryable<TResult>> orderBy,
            Action<IEnumerable<TResult>> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.FindInDataSourceAsync(selector, predicate, orderBy).OnComplete(onSuccess, onFail);
        }

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<IEnumerable> FindInDataSourceAsync<T>(
            this IRepository<T> source, Func<IQueryable<T>, IQueryable> selector, Expression<Func<T, bool>> predicate,
            Func<IQueryable, IOrderedQueryable> orderBy, Action<IEnumerable> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.FindInDataSourceAsync(selector, predicate, orderBy).OnComplete(onSuccess, onFail);
        }

        // Legacy support for IUnitOfWork

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult<SaveResult> CommitAsync(
            this IUnitOfWork source, Action<SaveResult> onSuccess, Action<Exception> onFail)
        {
            return source.CommitAsync().OnComplete(onSuccess, onFail);
        }

        // Legacy support for INavigator

        /// <summary>
        /// Provides compatibility for legacy code.
        /// </summary>
        public static OperationResult NavigateToAsync<T>(
            this INavigator source, Action<T> prepare, Func<T, IResult> prepareTargetAsync)
        {
            if (prepare != null && prepareTargetAsync != null)
                throw new ArgumentException(StringResources.CannotPrepareSyncAndAsync);
            if (prepare != null)
                return source.NavigateToAsync(prepare);
            if (prepareTargetAsync != null)
                return source.NavigateToAsync<T>(target => prepareTargetAsync(target).AsTask());

            return source.NavigateToAsync<T>();
        }
    }
}