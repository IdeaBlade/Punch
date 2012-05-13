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

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Caliburn.Micro;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    public static partial class Composition
    {
        private static CompositionContainer _container;
        private static ComposablePartCatalog _catalog;

        static Composition()
        {
            EntityManager.EntityManagerCreated += OnEntityManagerCreated;
        }

        private static void OnEntityManagerCreated(object sender, EntityManagerCreatedEventArgs args)
        {
            if (!args.EntityManager.IsClient)
                return;

            var locator = new PartLocator<IAuthenticationService>(
                CreationPolicy.Shared, () => args.EntityManager.CompositionContext);
            if (locator.IsAvailable)
                args.EntityManager.AuthenticationContext = locator.GetPart().AuthenticationContext;
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

        /// <summary>Satisfies all the imports on the provided instance.</summary>
        /// <param name="instance">The instance for which to satisfy the MEF imports.</param>
        public static void BuildUp(object instance)
        {
            // Skip if in design mode.
            if (Execute.InDesignMode)
                return;

            Container.SatisfyImportsOnce(instance);
        }

#if !SILVERLIGHT5

        /// <summary>
        /// Enables full design time support for the specified EntityManager type.
        /// </summary>
        /// <typeparam name="T">The type of EntityManager needing design time support.</typeparam>
        /// <remarks>This method must be called as early as possible, usually in the bootstrapper's static constructor.</remarks>
        public static void EnableDesignTimeSupport<T>() where T : EntityManager
        {
            if (Execute.InDesignMode)
            {
                string assemblyName = typeof(T).Assembly.FullName;
                if (IdeaBladeConfig.Instance.ProbeAssemblyNames.Contains(assemblyName))
                    return;

                IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(assemblyName);
            }
        }

#endif

        internal static void EnsureRequiredProbeAssemblies()
        {
            IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof(EntityManagerProvider<>).Assembly.FullName);
        }
    }
}