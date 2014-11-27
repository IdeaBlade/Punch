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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using IdeaBlade.Core;
using Action = System.Action;

namespace Cocktail
{
    /// <summary>
    ///   Abstract base class to configure the framework.
    /// </summary>
    public abstract class CocktailBootstrapper : BootstrapperBase
    {
        private Task _task;

        /// <summary>
        ///   Creates an instance of CocktailBootstrapper.
        /// </summary>
        /// <param name="useApplication"> Optionally specify if the bootstrapper should hook into the application object. </param>
        protected CocktailBootstrapper(bool useApplication = true)
            : base(useApplication)
        {
            DefaultDebugLogger.SetAsLogger();
            Initialize();
        }

        /// <summary>
        ///   Configures the framework and sets up the IoC container.
        /// </summary>
        protected override void Configure()
        {
            base.Configure();

            AddValueConverterConventions();
        }

        /// <summary>
        ///   Adds the stock <see cref="ValueConverterConvention" />s to the
        ///   <see cref="ValueConverterConventionRegistry" /> and thus to the
        ///   Caliburn <see cref="ConventionManager" />.
        /// </summary>
        protected virtual void AddValueConverterConventions()
        {
            ValueConverterConventionRegistry.AddConventionsToConventionManager();
            new PathToImageSourceConverter().RegisterConvention();
            new BinaryToImageSourceConverter().RegisterConvention();
        }

        /// <summary>
        ///   Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected override void StartRuntime()
        {
            base.StartRuntime();

            _task = StartRuntimeAsync();
        }

        /// <summary>
        ///   Provides an opportunity to perform asynchronous configuration at runtime.
        /// </summary>
        protected virtual Task StartRuntimeAsync()
        {
            return TaskFns.FromResult(true);
        }

        /// <summary>
        ///   Calls action when <see cref="StartRuntimeAsync" /> completes.
        /// </summary>
        /// <param name="completedAction"> Action to be performed when configuration completes. </param>
        protected async void WhenCompleted(Action completedAction)
        {
            await _task;
            completedAction();
        }

        /// <summary>
        ///   Locates the supplied service.
        /// </summary>
        /// <param name="serviceType"> The service to locate. </param>
        /// <param name="key"> The key to locate. </param>
        /// <returns> The located service. </returns>
        protected override object GetInstance(Type serviceType, string key)
        {
            return Composition.GetInstance(serviceType, key);
        }

        /// <summary>
        ///   Locates all instances of the supplied service.
        /// </summary>
        /// <param name="serviceType"> The service to locate. </param>
        /// <returns> The located services. </returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Composition.GetInstances(serviceType, null);
        }

        /// <summary>
        ///   Performs injection on the supplied instance.
        /// </summary>
        /// <param name="instance"> The instance to perform injection on. </param>
        protected override void BuildUp(object instance)
        {
            Composition.BuildUp(instance);
        }
    }

    /// <summary>
    ///   Abstract base class to configure the framework to use MEF as the application's IoC container.
    /// </summary>
    public abstract class CocktailMefBootstrapper : CocktailBootstrapper
    {
        private MefCompositionProvider _compositionProvider;

        /// <summary>
        ///   Creates an instance of CocktailMefBootstrapper.
        /// </summary>
        /// <param name="useApplication"> Optionally specify if the bootstrapper should hook into the application object. </param>
        protected CocktailMefBootstrapper(bool useApplication = true)
            : base(useApplication)
        {
        }

        /// <summary>
        ///   Override to add additional exports to the CompositionHost during configuration.
        /// </summary>
        /// <param name="batch"> The composition batch to add to. </param>
        protected virtual void PrepareCompositionContainer(CompositionBatch batch)
        {
            if (!_compositionProvider.IsTypeRegistered<IWindowManager>())
                batch.AddExportedValue<IWindowManager>(new WindowManager());

            if (!_compositionProvider.IsTypeRegistered<IDialogManager>())
                batch.AddExportedValue<IDialogManager>(new DialogManager());
        }

        /// <summary>
        ///   Override to substitute the default composition catalog with a custom catalog.
        /// </summary>
        /// <returns> Return the custom catalog that should be used by Cocktail to get access to MEF exports. </returns>
        protected virtual ComposablePartCatalog PrepareCompositionCatalog()
        {
            return _compositionProvider.DefaultCatalog;
        }

        /// <summary>
        ///   Configures the framework and sets up the IoC container.
        /// </summary>
        protected override void Configure()
        {
            base.Configure();

            EnsureBootstrapperHasNoExports();

            _compositionProvider = new MefCompositionProvider();
            _compositionProvider.Configure(catalog: PrepareCompositionCatalog());
            var batch = new CompositionBatch();
            PrepareCompositionContainer(batch);
            _compositionProvider.Compose(batch);
            Composition.SetProvider(_compositionProvider);
            OnCatalogRecomposed();
            _compositionProvider.Recomposed += (s, args) => OnCatalogRecomposed();
        }

        /// <summary>
        ///   Ensures that no MEF ExportAttributes are used in the Bootstrapper
        /// </summary>
        private void EnsureBootstrapperHasNoExports()
        {
            var type = GetType();

            // Throw exception if class is decorated with ExportAttribute
            if (type.GetCustomAttributes(typeof(ExportAttribute), true).Any())
                throw new CompositionException(StringResources.BootstrapperMustNotBeDecoratedWithExports);

            // Throw exception if any of the class members are decorated with ExportAttribute
            if (type.GetMembers().Any(m => m.GetCustomAttributes(typeof(ExportAttribute), true).Any()))
                throw new CompositionException(StringResources.BootstrapperMustNotBeDecoratedWithExports);
        }

        private void OnCatalogRecomposed()
        {
            UpdateAssemblySourceFromCatalog(_compositionProvider.DefaultCatalog);

            // The Bootstrapper is not owned by the container, so it doesn't automatically recompose
            BuildUp(this);
        }

        private void UpdateAssemblySourceFromCatalog(ComposablePartCatalog catalog)
        {
            if (catalog is AggregateCatalog)
                UpdateAssemblySourceFromCatalog(catalog as AggregateCatalog);

            if (catalog is AssemblyCatalog)
                UpdateAssemblySourceFromCatalog(catalog as AssemblyCatalog);
        }

        private void UpdateAssemblySourceFromCatalog(AggregateCatalog catalog)
        {
            catalog.Catalogs.ForEach(UpdateAssemblySourceFromCatalog);
        }

        private void UpdateAssemblySourceFromCatalog(AssemblyCatalog catalog)
        {
            if (AssemblySource.Instance.Contains(catalog.Assembly))
                return;

            AssemblySource.Instance.Add(catalog.Assembly);
        }
    }

    /// <summary>
    ///   Abstract base class to configure the framework to use MEF as the application's IoC container and launch the root ViewModel.
    /// </summary>
    /// <typeparam name="TRootModel"> The ViewModel of the main screen. </typeparam>
    public class CocktailMefBootstrapper<TRootModel> : CocktailMefBootstrapper
    {
        /// <summary>
        ///   Creates an instance of the framework bootstrapper.
        /// </summary>
        /// <param name="useApplication"> Optionally specify if the bootstrapper should hook into the application object. </param>
        public CocktailMefBootstrapper(bool useApplication = true)
            : base(useApplication)
        {
        }

        /// <summary>
        ///   Performs the framework startup sequence.
        /// </summary>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            WhenCompleted(() => DisplayRootViewFor(typeof(TRootModel)));
        }
    }
}