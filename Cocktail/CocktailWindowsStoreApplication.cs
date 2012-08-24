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

using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace Cocktail
{
    public abstract class CocktailWindowsStoreApplication : CaliburnApplication
    {
        private readonly Type _rootViewType;

        static CocktailWindowsStoreApplication()
        {
            DefaultDebugLogger.SetAsLogger();
        }

        public CocktailWindowsStoreApplication(Type rootViewType)
        {
            _rootViewType = rootViewType;
        }

        protected override Type GetDefaultView()
        {
            return _rootViewType;
        }

        /// <summary>Locates the supplied service.</summary>
        /// <param name="serviceType">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        protected override object GetInstance(Type serviceType, string key)
        {
            return Composition.GetInstance(serviceType, key);
        }

        /// <summary>Locates all instances of the supplied service.</summary>
        /// <param name="serviceType">The service to locate.</param>
        /// <returns>The located services.</returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Composition.GetInstances(serviceType, null);
        }

        /// <summary>Performs injection on the supplied instance.</summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            Composition.BuildUp(instance);
        }
    }

    public abstract class CocktailMefWindowsStoreApplication : CocktailWindowsStoreApplication
    {
        private readonly MefCompositionProvider _compositionProvider;

        static CocktailMefWindowsStoreApplication()
        {
            MefCompositionProvider.EnsureRequiredProbeAssemblies();
        }

        public CocktailMefWindowsStoreApplication(Type rootViewType) : base(rootViewType)
        {
            _compositionProvider = new MefCompositionProvider();
        }

        protected virtual void PrepareConventions(ConventionBuilder conventions)
        {
            conventions
                .ForTypesMatching(type => type.Name.EndsWith("ViewModel"))
                .Export();
        }

        protected override void Configure()
        {
            base.Configure();

            EnsureBootstrapperHasNoExports();

            var conventions = new ConventionBuilder();
            PrepareConventions(conventions);
            _compositionProvider.Configure(conventions);
            Composition.SetProvider(_compositionProvider);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return IdeaBlade.Core.Composition.CompositionHost.Instance.ProbeAssemblies;
        }

        /// <summary>
        /// Ensures that no MEF ExportAttributes are used in the Bootstrapper
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
