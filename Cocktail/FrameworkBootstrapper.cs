//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// Abstract base class for the FrameworkBootstrapper
    /// </summary>
    public abstract class FrameworkBootstrapper : Bootstrapper
    {
        /// <summary>
        /// Static initialization
        /// </summary>
        static FrameworkBootstrapper()
        {
            DefaultDebugLogger.SetAsLogger();
            Composition.EnsureRequiredProbeAssemblies();
        }

        /// <summary>
        /// Creates an instance of FrameworkBootstrapper.
        /// </summary>
        /// <param name="useApplication">Optionally specify if the bootstrapper should hook into the application object.</param>
        protected FrameworkBootstrapper(bool useApplication = true)
            : base(useApplication)
        {
        }

        /// <summary>Override to add additional exports to the CompositionHost during configuration.</summary>
        /// <param name="batch">The composition batch to add to.</param>
        protected virtual void PrepareCompositionContainer(CompositionBatch batch)
        {
            if (!Composition.ExportExists<IWindowManager>())
                batch.AddExportedValue<IWindowManager>(new WindowManager());
        }

        /// <summary>
        /// Ensures that no MEF ExportAttributes are used in the Bootstrapper
        /// </summary>
        private void EnsureBootstrapperHasNoExports()
        {
            Type type = GetType();

            // Throw exception if class is decorated with ExportAttribute
            if (type.GetCustomAttributes(typeof(ExportAttribute), true).Any())
                throw new CompositionException(StringResources.BootstrapperMustNotBeDecoratedWithExports);

            // Throw exception if any of the class members are decorated with ExportAttribute
            if (type.GetMembers().Any(m => m.GetCustomAttributes(typeof(ExportAttribute), true).Any()))
                throw new CompositionException(StringResources.BootstrapperMustNotBeDecoratedWithExports);
        }

        /// <summary>Configures the framework.</summary>
        protected override void Configure()
        {
            EnsureBootstrapperHasNoExports();

            var batch = new CompositionBatch();
            PrepareCompositionContainer(batch);
            Composition.Configure(batch);
            UpdateAssemblySource();
            Composition.Recomposed += (s, args) => UpdateAssemblySource();
            AddValueConverterConventions(); 
        }

        private void UpdateAssemblySource()
        {
            IObservableCollection<Assembly> assemblySource = AssemblySource.Instance;
            IEnumerable<Assembly> assemblies = Composition.Catalog.Catalogs.OfType<AssemblyCatalog>()
                .Select(c => c.Assembly)
                .Where(a => !assemblySource.Contains(a));

            assemblySource.AddRange(assemblies);

            // The Bootstrapper is not owned by the container, so it doesn't automatically recompose
            BuildUp(this);
        }

        /// <summary>
        /// Adds the stock <see cref="ValueConverterConvention"/>s to the
        /// <see cref="ValueConverterConventionRegistry"/> and thus to the
        /// Caliburn <see cref="ConventionManager"/>.
        /// </summary>
        protected virtual void AddValueConverterConventions()
        {
            ValueConverterConventionRegistry.AddConventionsToConventionManager();
            new PathToImageSourceConverter().RegisterConvention();
            new BinaryToImageSourceConverter().RegisterConvention(); 
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
            return Composition.GetInstances(serviceType);
        }

        /// <summary>Performs injection on the supplied instance.</summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            Composition.BuildUp(instance);
        }
    }

    /// <summary>Extend from FrameworkBootstrapper&lt;TRootModel&gt; to create your own Application Bootstrapper.</summary>
    /// <typeparam name="TRootModel">The ViewModel for the main screen.</typeparam>
    /// <example>
    /// 	<code title="Development Harness Bootstrapper" description="Demonstrates how to create a Bootstrapper for a Development Harness" lang="CS">
    /// public class AppBootstrapper : FrameworkBootstrapper&lt;HarnessViewModel&gt;
    /// {
    ///     // Add additional logic if required. 
    /// }</code>
    /// 	<code title="App.xaml" description="Demonstrates how to add the Bootstrapper as a static resource to trigger the bootstrapping of the application." lang="XAML">
    /// &lt;Application x:Class="SampleApplication.App" 
    ///              xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    ///              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    ///              xmlns:app="clr-namespace:SampleApplication;assembly=SampleApplication.Harness.SL"&gt;
    ///     &lt;Application.Resources&gt;
    ///         &lt;app:AppBootstrapper x:Key="Bootstrapper" /&gt;
    ///     &lt;/Application.Resources&gt;
    /// &lt;/Application&gt;</code>
    /// </example>
    public class FrameworkBootstrapper<TRootModel> : FrameworkBootstrapper
    {
        /// <summary>
        /// Creates an instance of the framework bootstrapper.
        /// </summary>
        /// <param name="useApplication">Optionally specify if the bootstrapper should hook into the application object.</param>
        public FrameworkBootstrapper(bool useApplication = true)
            : base(useApplication)
        {
        }

        /// <summary>Performs the framework startup sequence.</summary>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

#if SILVERLIGHT
            DisplayRootViewFor(Application, typeof (TRootModel));
#else
            DisplayRootViewFor(typeof (TRootModel));
#endif
        }
    }
}