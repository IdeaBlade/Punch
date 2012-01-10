//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
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
using Caliburn.Micro.Extensions.Logging;
using IdeaBlade.Application.Framework.Core.Composition;
using IdeaBlade.Application.Framework.Core.Persistence;
using IdeaBlade.Core;
using IdeaBlade.EntityModel;
using CompositionHost = IdeaBlade.Core.Composition.CompositionHost;

namespace Caliburn.Micro.Extensions
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
            string logTypeName = LogManager.GetLog(typeof (object)).GetType().FullName;
            if (logTypeName == null || logTypeName.StartsWith("Caliburn.Micro.LogManager"))
                LogManager.GetLog = type => new CaliburnMicroDebugLogger(type);

            // Ensure that the following assemblies are always probed.
            IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof (FrameworkBootstrapper<>).Assembly.FullName);
            IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof (BaseEntityManagerProvider<>).Assembly.FullName);
        }

        /// <summary>
        /// Creates an instance of the framework bootstrapper.
        /// </summary>
        /// <param name="useApplication">Optionally specify if the bootstrapper should hook into the application object.</param>
        protected FrameworkBootstrapper(bool useApplication = true) : base(useApplication)
        {
        }

        /// <summary>
        /// Indicates to the framework, that the provided EntityManager type makes use of the DevForce Fake Backing Store.
        /// </summary>
        /// <remarks>By registering the EntityManager type with this method, the framework makes sure,
        /// that the fake backing store gets initialized. The fake backing store is either initialized by
        /// the development harness or before login, if you are not running the harness. If the application
        /// doesn't login first, the fake backing store must be manually initialized.</remarks>
        /// <typeparam name="T"></typeparam>
        protected static void UsesFakeStore<T>() where T : EntityManager
        {
            if (Execute.InDesignMode)
            {
                // Must be called before the first EM gets created
                // This allows sample data to be deserialzied from a cache file at design time
                IdeaBladeConfig.Instance.ProbeAssemblyNames.Add(typeof (T).Assembly.FullName);
            }

            FakeBackingStoreManager.Instance.Register<T>();
        }

        /// <summary>Override to add additional exports to the CompositionHost during configuration.</summary>
        /// <param name="batch">The composition batch to add to.</param>
        protected virtual void InitializeCompositionBatch(CompositionBatch batch)
        {
            if (!CompositionHelper.ExportExists<IWindowManager>())
                batch.AddExportedValue<IWindowManager>(new WindowManager());

            if (!CompositionHelper.ExportExists<IEventAggregator>())
                batch.AddExportedValue<IEventAggregator>(new EventAggregator());
        }

        /// <summary>
        /// Ensures that no MEF ExportAttributes are used in the Bootstrapper
        /// </summary>
        private void ValidateBootstrapper()
        {
            Type type = GetType();

            // Throw exception if class is decorated with ExportAttribute
            if (ReflectionFns.GetAttribute(type, typeof (ExportAttribute)) != null)
                throw new CompositionException(StringResources.BootstrapperMustNotBeDecoratedWithExports);

            // Throw exception if any of the class members are decorated with ExportAttribute
            if (type.GetMembers().Any(m => ReflectionFns.GetAttribute(m, typeof (ExportAttribute)) != null))
                throw new CompositionException(StringResources.BootstrapperMustNotBeDecoratedWithExports);
        }

        /// <summary>
        /// Ensures that all the required assemblies were probed
        /// </summary>
        private void CheckRequiredProbeAssemblies()
        {
            var requiredAssemblies = new[]
                                         {
                                             typeof (FrameworkBootstrapper<>).Assembly,
                                             typeof (BaseEntityManagerProvider<>).Assembly
                                         };
            IEnumerable<Assembly> assemblies =
                CompositionHelper.Catalog.Catalogs.OfType<AssemblyCatalog>().Select(c => c.Assembly);

            if (requiredAssemblies.All(assemblies.Contains)) return;

            throw new CompositionException(
                string.Format(StringResources.MissingRequiredProbeAssemblies,
                              string.Join(",", requiredAssemblies.Select(a => string.Format("[{0}]", a.FullName)))));
        }

        /// <summary>Configures the framework.</summary>
        protected override void Configure()
        {
            // Nothing to configure in design mode
            if (Execute.InDesignMode) return;

            ValidateBootstrapper();

            var batch = new CompositionBatch();
            InitializeCompositionBatch(batch);

            CompositionHelper.Configure(batch);
            CheckRequiredProbeAssemblies();

            CompositionHost.Recomposed += RefreshCaliburnAssemblySource;
            RefreshCaliburnAssemblySource(CompositionHost.Instance, EventArgs.Empty);

            // The bootstrapper was created outside of the container. Manually satisfy the imports.
            BuildUp(this);

            // Caliburn's new RegEx based ViewLocator no longer finds views for the <Namespace>.ViewModel.<BaseName>ViewModel construct
            // Add rule to support above construct
            ViewLocator.NameTransformer.AddRule
                (
                    @"(?<namespace>(.*\.)*)ViewModel\.(?<basename>[A-Za-z]\w*)(?<suffix>ViewModel$)",
                    @"${namespace}View.${basename}View",
                    @"(.*\.)*ViewModel\.[A-Za-z]\w*ViewModel$"
                );
        }

        private void RefreshCaliburnAssemblySource(object sender, EventArgs e)
        {
            IObservableCollection<Assembly> assemblySource = AssemblySource.Instance;
            IEnumerable<Assembly> assemblies =
                CompositionHelper.Catalog.Catalogs.OfType<AssemblyCatalog>().Select(c => c.Assembly).Where(
                    a => !assemblySource.Contains(a));

            assemblySource.AddRange(assemblies);

            // The Bootstrapper is not owned by the container, so it doesn't automatically recompose
            BuildUp(this);
        }

        /// <summary>Locates the supplied service.</summary>
        /// <param name="serviceType">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        protected override object GetInstance(Type serviceType, string key)
        {
            return CompositionHelper.GetInstance(serviceType, key);
        }

        /// <summary>Locates all instances of the supplied service.</summary>
        /// <param name="serviceType">The service to locate.</param>
        /// <returns>The located services.</returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            // Skip when in design mode
            return Execute.InDesignMode ? new object[0] : CompositionHelper.GetInstances(serviceType);
        }

        /// <summary>Performs injection on the supplied instance.</summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            // Skip when in design mode
            if (Execute.InDesignMode) return;

            CompositionHelper.BuildUp(instance);
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

    /// <summary>
    /// Obsolete.
    /// </summary>
    /// <typeparam name="TRootModel"></typeparam>
    /// <typeparam name="TEntityManager"></typeparam>
    [Obsolete("Use FrameworkBootstrapper<TRootModel> instead!")]
    public class FrameworkBootstrapper<TRootModel, TEntityManager> : FrameworkBootstrapper<TRootModel>
        where TEntityManager : EntityManager
    {
        static FrameworkBootstrapper()
        {
            UsesFakeStore<TEntityManager>();
        }
    }
}