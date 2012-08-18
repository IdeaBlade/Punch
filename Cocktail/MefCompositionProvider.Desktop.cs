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

using Caliburn.Micro;
using IdeaBlade.Core.Composition;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    internal partial class MefCompositionProvider
    {
        private CompositionContainer _container;
        private ComposablePartCatalog _catalog;

        /// <summary>
        ///   Returns the current catalog in use.
        /// </summary>
        /// <returns> Unless a custom catalog is provided through <see cref="Configure" />, this property returns <see cref="DefaultCatalog" /> </returns>
        public ComposablePartCatalog Catalog
        {
            get { return _catalog ?? DefaultCatalog; }
        }

        /// <summary>
        ///   Returns the default catalog in use by DevForce.
        /// </summary>
        public ComposablePartCatalog DefaultCatalog
        {
            get { return CompositionHost.Instance.Container.Catalog; }
        }

        /// <summary>
        ///   Returns the CompositionContainer in use.
        /// </summary>
        public CompositionContainer Container
        {
            get { return _container ?? (_container = new CompositionContainer(Catalog)); }
        }

        /// <summary>
        ///   Configures the CompositionHost.
        /// </summary>
        /// <param name="compositionBatch"> Optional changes to the <see cref="CompositionContainer" /> to include during the composition. </param>
        /// <param name="catalog"> The custom catalog to be used by Cocktail to get access to MEF exports. </param>
        public void Configure(CompositionBatch compositionBatch = null, ComposablePartCatalog catalog = null)
        {
            _catalog = catalog;

            var batch = compositionBatch ?? new CompositionBatch();
            if (!IsTypeRegistered<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            Compose(batch);
        }

        /// <summary>
        ///   Fired when the composition container is modified after initialization.
        /// </summary>
        public event EventHandler<RecomposedEventArgs> Recomposed
        {
            add { CompositionHost.Recomposed += value; }
            remove { CompositionHost.Recomposed -= value; }
        }
    }
}
