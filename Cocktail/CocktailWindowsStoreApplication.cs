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
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Cocktail
{
    /// <summary>
    /// Application base class for a Windows 8 Store app.
    /// </summary>
    public abstract class CocktailWindowsStoreApplication : CaliburnApplication
    {
        private readonly Type _rootViewType;

        static CocktailWindowsStoreApplication()
        {
            DefaultDebugLogger.SetAsLogger();
        }

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="rootViewType">The type of the root view.</param>
        protected CocktailWindowsStoreApplication(Type rootViewType)
        {
            _rootViewType = rootViewType;
        }

        /// <summary>
        /// Returns the root view type.
        /// </summary>
        protected override Type GetDefaultView()
        {
            return _rootViewType;
        }

        /// <summary>
        ///   Provides an opportunity to perform asynchronous configuration at runtime.
        /// </summary>
        protected virtual Task StartRuntimeAsync()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        ///   Initializes application and displays the root view
        /// </summary>
        protected override async void EnsurePage(IActivatedEventArgs args)
        {
            // This logic is copied from the base class and modified to support async bootstrapping
            Initialise();
            await StartRuntimeAsync();

            switch (args.Kind)
            {
                default:

                    var defaultView = GetDefaultView();

                    RootFrame.Navigate(defaultView);

                    break;
            }

            // Seems stupid but observed weird behaviour when resetting the Content
            if (Window.Current.Content != RootFrame)
                Window.Current.Content = RootFrame;

            Window.Current.Activate();
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
    /// Application base class for a Windows 8 Store app that uses MEF as the IoC implementation.
    /// </summary>
    public abstract class CocktailMefWindowsStoreApplication : CocktailWindowsStoreApplication
    {
        private readonly MefCompositionProvider _compositionProvider;

        /// <summary>
        /// Initializes the application object.
        /// </summary>
        /// <param name="rootViewType">The type of the root view.</param>
        protected CocktailMefWindowsStoreApplication(Type rootViewType) : base(rootViewType)
        {
            _compositionProvider = new MefCompositionProvider();
        }

        /// <summary>
        /// Override to setup up MEF export conventions.
        /// </summary>
        /// <param name="conventions"></param>
        protected virtual void PrepareConventions(ConventionBuilder conventions)
        {
            // Automatic export of ViewModels.
            conventions
                .ForTypesMatching(type => type.Name.EndsWith("ViewModel"))
                .Export();
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            base.Configure();

            EnsureBootstrapperHasNoExports();

            var conventions = new ConventionBuilder();
            PrepareConventions(conventions);

            _compositionProvider.Configure(conventions);
            Composition.SetProvider(_compositionProvider);

            AddExportedValue<INavigator>(new Navigator(RootFrame));
            BuildUp(this);
        }

        /// <summary>
        ///   Adds a singleton instance to the composition container.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="value"> </param>
        protected void AddExportedValue<T>(T value)
        {
            _compositionProvider.AddExportedValue(value);
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>
        /// A list of assemblies to inspect.
        /// </returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return CompositionHost.Instance.ProbeAssemblies;
        }

        /// <summary>
        ///   Ensures that no MEF ExportAttributes are used in the Bootstrapper
        /// </summary>
        private void EnsureBootstrapperHasNoExports()
        {
            var type = GetType().GetTypeInfo();

            // Throw exception if class is decorated with ExportAttribute
            if (type.GetCustomAttributes(typeof(ExportAttribute), true).Any())
                throw new CompositionFailedException(StringResources.BootstrapperMustNotBeDecoratedWithExports);

            // Throw exception if any of the class members are decorated with ExportAttribute
            if (type.DeclaredMembers.Any(m => m.GetCustomAttributes(typeof(ExportAttribute), true).Any()))
                throw new CompositionFailedException(StringResources.BootstrapperMustNotBeDecoratedWithExports);
        }
    }
}