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
using IdeaBlade.Core.Composition;

namespace Cocktail
{
    internal partial class MefCompositionProvider
    {
        private readonly Dictionary<string, XapDownloadOperation> _xapDownloadOperations =
            new Dictionary<string, XapDownloadOperation>();

        /// <summary>Asynchronously downloads a XAP file and adds all exported parts to the catalog.</summary>
        /// <param name="relativeUri">The relative URI for the XAP file to be downloaded.</param>
        /// <returns>
        ///     The asynchronous download <see cref="Task" />.
        /// </returns>
        public Task AddXapAsync(string relativeUri)
        {
            XapDownloadOperation operation;
            if (_xapDownloadOperations.TryGetValue(relativeUri, out operation) && !operation.Task.IsFaulted)
                return operation.Task;

            operation = _xapDownloadOperations[relativeUri] = new XapDownloadOperation(relativeUri, this);
            return operation.Task;
        }
    }

    internal class XapDownloadOperation
    {
        private readonly MefCompositionProvider _compositionProvider;
        private readonly TaskCompletionSource<bool> _tcs;
        private readonly DynamicXap _xap;

        public XapDownloadOperation(string xapUri, MefCompositionProvider compositionProvider)
        {
            _compositionProvider = compositionProvider;
            _tcs = new TaskCompletionSource<bool>();
            _xap = new DynamicXap(new Uri(xapUri, UriKind.Relative));
            _xap.Loaded += (s, args) => XapDownloadCompleted(args);
        }

        public Task Task
        {
            get { return _tcs.Task; }
        }

        private void XapDownloadCompleted(DynamicXapLoadedEventArgs args)
        {
            if (args.Cancelled)
            {
                _tcs.SetCanceled();
                return;
            }

            if (!args.HasError)
            {
                _compositionProvider.IsRecomposing = true;
                try
                {
                    CompositionHost.Recomposed += OnRecomposed;
                    CompositionHost.Add(_xap);
                    _tcs.TrySetResult(true);
                }
                catch (Exception e)
                {
                    _tcs.TrySetException(e);
                }
                finally
                {
                    _compositionProvider.IsRecomposing = false;
                }
            }
            else
                _tcs.SetException(args.Error);
        }

        private void OnRecomposed(object sender, RecomposedEventArgs args)
        {
            if (args.HasError)
                _tcs.TrySetException(args.XapLoadError);

            CompositionHost.Recomposed -= OnRecomposed;
        }
    }
}