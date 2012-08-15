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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Caliburn.Micro;
using IdeaBlade.Core.Composition;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    public static partial class Composition
    {
        private static CompositionContainer _container;
        private static ComposablePartCatalog _catalog;

        internal static bool IsRecomposing { get; set; }

        /// <summary>
        ///   Returns the current catalog in use.
        /// </summary>
        /// <returns> Unless a custom catalog is provided through <see cref="Configure" />, this property returns <see cref="DefaultCatalog" /> </returns>
        public static ComposablePartCatalog Catalog
        {
            get { return _catalog ?? DefaultCatalog; }
        }

        /// <summary>
        ///   Returns the default catalog in use by DevForce.
        /// </summary>
        public static ComposablePartCatalog DefaultCatalog
        {
            get { return CompositionHost.Instance.Container.Catalog; }
        }

        /// <summary>
        ///   Returns the CompositionContainer in use.
        /// </summary>
        public static CompositionContainer Container
        {
            get { return _container ?? (_container = new CompositionContainer(Catalog)); }
        }

        /// <summary>
        ///   Configures the CompositionHost.
        /// </summary>
        /// <param name="compositionBatch"> Optional changes to the <see cref="CompositionContainer" /> to include during the composition. </param>
        /// <param name="catalog"> The custom catalog to be used by Cocktail to get access to MEF exports. </param>
        public static void Configure(CompositionBatch compositionBatch = null, ComposablePartCatalog catalog = null)
        {
            Clear();
            _catalog = catalog;

            var batch = compositionBatch ?? new CompositionBatch();
            if (!ExportExists<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            Compose(batch);
        }

        /// <summary>
        ///   Resets the CompositionContainer to it's initial state.
        /// </summary>
        /// <remarks>
        ///   After calling <see cref="Clear" /> , <see cref="Configure" /> should be called to configure the new CompositionContainer.
        /// </remarks>
        public static void Clear()
        {
            if (_container != null)
                _container.Dispose();
            _container = null;
            _catalog = null;

            Cleared(null, EventArgs.Empty);
        }

        /// <summary>
        ///   Fired when the composition container is modified after initialization.
        /// </summary>
        public static event EventHandler<RecomposedEventArgs> Recomposed
        {
            add { CompositionHost.Recomposed += value; }
            remove { CompositionHost.Recomposed -= value; }
        }
    }
}
