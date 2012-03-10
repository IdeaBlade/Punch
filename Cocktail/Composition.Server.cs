//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    /// <summary>
    /// Sets up a composition container and provides means to interact with the container.
    /// </summary>
    public static class Composition
    {
        [ThreadStatic] 
        private static CompositionHelper _compositionHelper;

        private static CompositionHelper CompositionHelper
        {
            get { return _compositionHelper ?? (_compositionHelper = new CompositionHelper()); }
        }

        /// <summary>Returns the current catalog in use.</summary>
        /// <returns>Unless a custom catalog is provided through <see cref="Configure"/>, this property returns <see cref="AggregateCatalog"/></returns>
        public static ComposablePartCatalog Catalog
        {
            get { return CompositionHelper.Catalog; }
        }

        /// <summary>
        /// Returns the AggregateCatalog in use by DevForce.
        /// </summary>
        public static AggregateCatalog AggregateCatalog
        {
            get { return CompositionHelper.AggregateCatalog; }
        }

        /// <summary>Returns the CompositionContainer in use.</summary>
        public static CompositionContainer Container
        {
            get { return CompositionHelper.Container; }
        }

        /// <summary>Configures the CompositionHost.</summary>
        /// <param name="compositionBatch">
        ///     Optional changes to the <span><see cref="CompositionContainer"/></span> to include during the composition.
        /// </param>
        /// <param name="catalog">The custom catalog to be used by Cocktail to get access to MEF exports.</param>
        public static void Configure(CompositionBatch compositionBatch = null, ComposablePartCatalog catalog = null)
        {
            CompositionHelper.Configure(catalog);

            CompositionBatch batch = compositionBatch ?? new CompositionBatch();
            Compose(batch);
        }

        /// <summary>Executes composition on the container, including the changes in the specified <see cref="CompositionBatch"/>.</summary>
        /// <param name="compositionBatch">
        /// 	Changes to the <see cref="CompositionContainer"/> to include during the composition.
        /// </param>
        public static void Compose(CompositionBatch compositionBatch)
        {
            CompositionHelper.Compose(compositionBatch);
        }

        /// <summary>
        /// Resets the CompositionContainer to it's initial state.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="Clear"/>, <see cref="Configure"/> should be called to configure the new CompositionContainer.
        /// </remarks>
        public static void Clear()
        {
            CompositionHelper.Clear();
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instance.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static T GetInstance<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstance<T>(requiredCreationPolicy);
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <typeparam name="T">Type of the requested instances.</typeparam>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public static IEnumerable<T> GetInstances<T>(CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstances<T>(requiredCreationPolicy);
        }

        /// <summary>
        /// 	<para>Returns an instance of the custom implementation for the provided type or contract name.</para>
        /// </summary>
        /// <param name="serviceType">The type of the requested instance.</param>
        /// <param name="key">The contract name of the instance requested. If no contract name is specified, the type will be used.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instance should be a shared, non-shared or any instance.</param>
        /// <returns>The requested instance.</returns>
        public static object GetInstance(Type serviceType, string key,
                                         CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstance(serviceType, key, requiredCreationPolicy);
        }

        /// <summary>
        /// 	<para>Returns all instances of the custom implementation for the provided type.</para>
        /// </summary>
        /// <param name="serviceType">Type of the requested instances.</param>
        /// <param name="requiredCreationPolicy">Optionally specify whether the returned instances should be shared, non-shared or any instances.</param>
        /// <returns>The requested instances.</returns>
        public static IEnumerable<object> GetInstances(Type serviceType,
                                                       CreationPolicy requiredCreationPolicy = CreationPolicy.Any)
        {
            return CompositionHelper.GetInstances(serviceType, requiredCreationPolicy);
        }

        /// <summary>Satisfies all the imports on the provided instance.</summary>
        /// <param name="instance">The instance for which to satisfy the MEF imports.</param>
        public static void BuildUp(object instance)
        {
            CompositionHelper.BuildUp(instance);
        }

        /// <summary>
        /// Fired when the composition container is modified after initialization.
        /// </summary>
        public static event EventHandler<RecomposedEventArgs> Recomposed
        {
            add { CompositionHost.Recomposed += value; }
            remove { CompositionHost.Recomposed -= value; }
        }

        /// <summary>
        /// Fired after <see cref="Clear"/> has been called to clear the current CompositionContainer.
        /// </summary>
        public static event EventHandler<EventArgs> Cleared
        {
            add { CompositionHelper.Cleared += value; }
            remove { CompositionHelper.Cleared -= value; }
        }

        internal static void EnsureRequiredProbeAssemblies()
        {
            IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof (EntityManagerProvider<>).Assembly.FullName);
        }

        internal static IEnumerable<Export> GetExportsCore(Type serviceType, string key, CreationPolicy policy)
        {
            return CompositionHelper.GetExportsCore(serviceType, key, policy);
        }

        internal static bool ExportExists<T>()
        {
            return CompositionHelper.ExportExists<T>();
        }
    }
}