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
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using Action = System.Action;

namespace Cocktail
{
    /// <summary>
    /// Abstract base class for the FrameworkBootstrapper
    /// </summary>
    public abstract class FrameworkBootstrapper : Bootstrapper
    {
        private bool _completed;
        private Action _completedActions;

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

            if (!Composition.ExportExists<IDialogManager>())
                batch.AddExportedValue<IDialogManager>(new DialogManager());
        }

        /// <summary>
        /// Called by the bootstrapper's constructor at runtime to start the framework.
        /// </summary>
        protected override void StartRuntime()
        {
            base.StartRuntime();

            ConfigureAsync().ToSequentialResult().Execute(OnComplete);
        }

        /// <summary>
        /// Configures the framework and sets up the IoC container.
        /// </summary>
        protected override void Configure()
        {
            base.Configure();

            EnsureBootstrapperHasNoExports();

            var batch = new CompositionBatch();
            PrepareCompositionContainer(batch);
            Composition.Configure(batch);
            UpdateAssemblySource();
            Composition.Recomposed += (s, args) => UpdateAssemblySource();
            AddValueConverterConventions();
        }

        /// <summary>
        /// Provides an opportunity to perform asynchronous configuration at runtime.
        /// </summary>
        protected virtual IEnumerable<IResult> ConfigureAsync()
        {
            yield return AlwaysCompletedOperationResult.Instance;
        }

        /// <summary>
        /// Calls action when <see cref="ConfigureAsync"/> completes. 
        /// </summary>
        /// <param name="completedAction">Action to be performed when configuration completes.</param>
        protected void WhenCompleted(Action completedAction)
        {
            if (completedAction == null) return;
            if (_completed)
            {
                completedAction();
                return;
            }
            _completedActions = (Action) Delegate.Combine(_completedActions, completedAction);
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

        private void OnComplete(ResultCompletionEventArgs args)
        {
            if (args.Error != null)
                throw args.Error;

            _completed = true;
            Action actions = _completedActions;
            _completedActions = null;
            if (actions == null) return;
            actions();
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

            WhenCompleted(
#if SILVERLIGHT
                () => DisplayRootViewFor(Application, typeof (TRootModel))
#else
                () => DisplayRootViewFor(typeof (TRootModel))
#endif
                );
        }
    }
}