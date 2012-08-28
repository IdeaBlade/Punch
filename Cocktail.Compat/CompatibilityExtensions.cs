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
using System.Threading;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    ///   Extension methods to provide support for the legacy asynchronous API.
    /// </summary>
    public static class CompatibilityExtensions
    {
        // Legacy support for IAuthenticationService

        public static OperationResult LoginAsync(
            this IAuthenticationService source, ILoginCredential credential, Action onSuccess, Action<Exception> onFail)
        {
            return source.LoginAsync(credential).OnComplete(onSuccess, onFail);
        }

        public static OperationResult LogoutAsync(this IAuthenticationService source, Action callback)
        {
            return source.LogoutAsync().OnComplete(callback, null);
        }

        // Legacy support for IDialogManager

        public static OperationResult<T> ShowDialogAsync<T>(
            this IDialogManager source, object content, IEnumerable<T> dialogButtons, string title)
        {
            return source.ShowDialogAsync(content, dialogButtons, title);
        }

        public static OperationResult<T> ShowDialogAsync<T>(
            this IDialogManager source, object content, T defaultButton, T cancelButton, IEnumerable<T> dialogButtons,
            string title)
        {
            return source.ShowDialogAsync(content, defaultButton, cancelButton, dialogButtons, title);
        }

        public static OperationResult<DialogResult> ShowDialogAsync(
            this IDialogManager source, object content, IEnumerable<DialogResult> dialogButtons, string title)
        {
            return source.ShowDialogAsync(content, dialogButtons, title);
        }

        public static OperationResult<T> ShowMessageAsync<T>(
            this IDialogManager source, string message, IEnumerable<T> dialogButtons, string title)
        {
            return source.ShowMessageAsync(message, dialogButtons, title);
        }

        public static OperationResult<T> ShowMessageAsync<T>(
            this IDialogManager source, string message, T defaultButton, T cancelButton, IEnumerable<T> dialogButtons,
            string title)
        {
            return source.ShowMessageAsync(message, defaultButton, cancelButton, dialogButtons, title);
        }

        public static OperationResult<DialogResult> ShowMessageAsync(
            this DialogManager source, string message, IEnumerable<DialogResult> dialogButtons, string title)
        {
            return source.ShowMessageAsync(message, dialogButtons, title);
        }

        // Legacy support for IFactory<T>

        public static OperationResult<T> CreateAsync<T>(
            this IFactory<T> source, Action<T> onSuccess, Action<Exception> onFail)
            where T : class
        {
            return source.CreateAsync().OnComplete(onSuccess, onFail);
        }

        // Legacy support for Fake Backing Store

        public static OperationResult InitializeFakeBackingStoreAsync<T>(
            this IEntityManagerProvider<T> source, Action onSuccess, Action<Exception> onFail)
            where T : EntityManager
        {
            return source.InitializeFakeBackingStoreAsync().OnComplete(onSuccess, onFail);
        }

        public static OperationResult ResetFakeBackingStoreAsync<T>(
            this IEntityManagerProvider<T> source, Action onSuccess, Action<Exception> onFail)
            where T : EntityManager
        {
            return source.ResetFakeBackingStoreAsync().OnComplete(onSuccess, onFail);
        }

        // Legacy support for IPager

        public static PageOperationResult<T> FirstPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.FirstPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        public static PageOperationResult<T> LastPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.LastPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        public static PageOperationResult<T> NextPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.NextPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        public static PageOperationResult<T> PreviousPageAsync<T>(
            this IPager<T> source, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.PreviousPageAsync(cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }

        public static PageOperationResult<T> GoToPageAsync<T>(
            this IPager<T> source, int pageIndex, Action<Page<T>> onSuccess, Action<Exception> onFail)
        {
            var cts = new CancellationTokenSource();
            var op = source.GoToPageAsync(pageIndex, cts.Token).OnComplete(onSuccess, onFail);
            return new PageOperationResult<T>(op, cts);
        }
    }
}